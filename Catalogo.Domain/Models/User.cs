namespace Catalogo.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } =  string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;
    
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public enum UserRole
    {
        User,
        Admin
    }
}   