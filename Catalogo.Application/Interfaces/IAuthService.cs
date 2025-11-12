using Catalogo.Domain.Models;

namespace Catalogo.Application.Interfaces;

public interface IAuthService
{
    Task<(string? AccessToken, string? RefreshToken)> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string username, string email, string password, string document, string role);
    Task<(string? AccessToken, string? RefreshToken)> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}