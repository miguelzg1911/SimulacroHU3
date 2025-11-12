using Xunit;
using Moq;
using Catalogo.Application.Services;
using Catalogo.Domain.Interfaces;
using Catalogo.Domain.Models;

public class UserServiceTests
{
    [Fact]
    public async Task CreateUser_ShouldAddUser_WhenValid()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User>());
        mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var service = new UserService(mockRepo.Object);

        var newUser = new User
        {
            Username = "testuser",
            Email = "test@mail.com",
            PasswordHash = "1234",
            Document = "111222333",
            Role = User.UserRole.User
        };

        // Act
        var result = await service.CreateAsync(newUser);

        // Assert
        Assert.Equal("testuser", result.Username);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}