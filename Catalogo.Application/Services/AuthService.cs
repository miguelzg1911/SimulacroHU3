using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Catalogo.Application.Interfaces;
using Catalogo.Domain.Interfaces;
using Catalogo.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Catalogo.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    // Login (MODIFICADO para Refresh Token)
    public async Task<(string? AccessToken, string? RefreshToken)> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null) return (null, null);  

        // Verificar hash
        bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!passwordValid) return (null, null);

        // Crear claims del token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("Document", user.Document),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), // Access Token de corta duración
            signingCredentials: cred
        );

        string accessTokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        // Generar y guardar Refresh Token
        var refreshTokenString = GenerateRefreshToken();
        user.RefreshToken = refreshTokenString;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh Token de larga duración (7 días)

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return (accessTokenString, refreshTokenString);
    }

    // Refresh Token
    public async Task<(string? AccessToken, string? RefreshToken)> RefreshAsync(string refreshToken)
    {
        var users = await _userRepository.GetAllUsersAsync();
        var existingUser = users.FirstOrDefault(u => u.RefreshToken == refreshToken);

        if (existingUser == null)
        {
            // El token no existe o ya fue usado/borrado
            return (null, null); 
        }

        // Verificar si el Refresh Token ha expirado
        if (existingUser.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            // Invalida el token expirado por seguridad
            existingUser.RefreshToken = null; 
            await _userRepository.SaveChangesAsync();
            return (null, null); 
        }

        // Si es válido, generar un nuevo Access Token (JWT)
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, existingUser.Username),
            new Claim(ClaimTypes.Email, existingUser.Email ?? string.Empty),
            new Claim("Document", existingUser.Document),
            new Claim(ClaimTypes.Role, existingUser.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var newToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), 
            signingCredentials: cred
        );
        string newAccessTokenString = new JwtSecurityTokenHandler().WriteToken(newToken);

        // Generar un NUEVO Refresh Token y actualizar la DB (ROTACIÓN DE TOKEN)
        var newRefreshTokenString = GenerateRefreshToken();
        existingUser.RefreshToken = newRefreshTokenString;
        existingUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 
        
        await _userRepository.UpdateAsync(existingUser);
        await _userRepository.SaveChangesAsync();

        return (newAccessTokenString, newRefreshTokenString);
    }

    // Registro
    public async Task<bool> RegisterAsync(string username, string email, string password, string document, string role)
    {
        var existing = await _userRepository.GetUserByEmailAsync(email);
        if (existing != null)
            throw new Exception("El correo ya está registrado");

        var newUser = new User
        {
            Username = username,
            Email = email,
            Document = document,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = Enum.TryParse<User.UserRole>(role, out var parsedRole) ? parsedRole : User.UserRole.User
        };

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    // Función de ayuda
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    // Logout
    public async Task LogoutAsync(string refreshToken)
    {
        var users = await _userRepository.GetAllUsersAsync();
        var user = users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = default;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
        }
    }
}