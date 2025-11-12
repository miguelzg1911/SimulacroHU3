using Catalogo.Application.Interfaces;
using Catalogo.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalogo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Registro de usuario
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] User user)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(user.Username) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.PasswordHash) ||
                string.IsNullOrWhiteSpace(user.Document) ||
                string.IsNullOrWhiteSpace(user.Role.ToString()))
            {
                return BadRequest("Todos los campos son obligatorios.");
            }

            await _authService.RegisterAsync(
                user.Username,
                user.Email,
                user.PasswordHash,
                user.Document,
                user.Role.ToString()
            );

            return Ok(new { message = "Usuario registrado correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Login de usuario
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] User user)
    {
        try
        {
            // El servicio ahora devuelve una tupla con ambos tokens
            var (accessToken, refreshToken) = await _authService.LoginAsync(user.Email, user.PasswordHash);

            if (accessToken == null || refreshToken == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            // Devolvemos el par de tokens
            return Ok(new { accessToken, refreshToken });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    // Refresh Token Rotation
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] string refreshToken)
    {
        // Verificar si se proporcionó un token
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return BadRequest(new { message = "Se requiere el Refresh Token." });
        }
        
        try
        {
            // Llamar al servicio para rotar el token
            var (newAccessToken, newRefreshToken) = await _authService.RefreshAsync(refreshToken);

            if (newAccessToken == null || newRefreshToken == null)
            {
                // Token inválido, expirado, o reutilizado (¡prueba de seguridad!)
                return Unauthorized(new { message = "Refresh Token inválido, expirado o revocado." });
            }

            // Devolver el nuevo par de tokens
            return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno al procesar el refresh token: " + ex.Message });
        }
    }

    // Logout (Cerrar Sesión)
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return BadRequest(new { message = "Se requiere el Refresh Token para cerrar la sesion." });
        }

        try
        {
            await _authService.LogoutAsync(refreshToken);
            return Ok(new { message = "Sesion cerrada correctamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno al cerrar la sesión. " } + ex.Message);
        }
    }
}