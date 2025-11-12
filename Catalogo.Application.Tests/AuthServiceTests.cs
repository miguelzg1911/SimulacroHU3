using Xunit;
using Moq;
using Catalogo.Application.Services;
using Catalogo.Domain.Interfaces;
using Catalogo.Domain.Models;
using Microsoft.Extensions.Configuration;

public class AuthServiceTests
{
    [Fact]
    public async Task Login_ShouldReturnToken_WhenValid()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Jwt:Key", "UnaContraseñaSecretaYMuyLargaAqui_12345"},
                {"Jwt:Issuer", "CatalogoApi"},
                {"Jwt:Audience", "CatalogoClient"}
            })
            .Build();

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("1234");

        mockRepo.Setup(r => r.GetUserByEmailAsync("user@mail.com"))
            .ReturnsAsync(new User { Id = 1, Username = "user", Email = "user@mail.com", PasswordHash = passwordHash, Role = User.UserRole.User });

        var service = new AuthService(mockRepo.Object, configuration);

        // Act
        var token = await service.LoginAsync("user@mail.com", "1234");

        // Assert
        Assert.NotNull(token);
    }
}