using Moq;
using TaskManagerAPI.Entities;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services;
using Xunit;

namespace TaskManagerAPI.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task CreateUserAsync_WithValidData_ReturnsUserDto()
        {
            ///// Arrange
            var mockRepository = new Mock<IUserRepository>();
            var userService = new UserService(mockRepository.Object);

            var createUserDto = new CreateUserDto
            {
                Username = "testuser",
                Email = "test@yahoo.com",
                Password = "password123",
                Role = "User"
            };

            ///// Mock--- username doesn't exist
            mockRepository
                .Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            ///// Mock--- AddAsync - use Callback to set ID
            mockRepository
                .Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Callback<User>(user => user.Id = 1)
                .Returns(Task.CompletedTask);

            ///// Act
            var result = await userService.CreateUserAsync(createUserDto);

            ///////Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@gmail.com", result.Email);
            Assert.Equal("User", result.Role);

            /////verify repository was called
            mockRepository.Verify(
                repo => repo.AddAsync(It.IsAny<User>()),
                Times.Once
            );
        }
    }
}