using Moq;
using TaskManagerAPI.Entities;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services;
using Xunit;

namespace TaskManagerAPI.Tests
{
    /// <summary>
    /// Unit tests for the UserService.
    /// Tests user creation, validation, and business logic.
    /// </summary>
    public class UserServiceTests
    {
      
        /// Verifies that a user is created successfully with valid data
     
        [Fact]
        public async Task CreateUserAsync_WithValidData_ReturnsUserDto()
        {
            ///// Arrange - Set up test data and dependencies
            var mockUserRepository = new Mock<IUserRepository>();
            var userService = new UserService(mockUserRepository.Object);

            var createUserDto = new CreateUserDto
            {
                Username = "testuser",
                Email = "test@yahoo.com",
                Password = "password123",
                Role = "User"
            };

            //////// Configure mock: Username doesn't exist (available)
            mockUserRepository
                .Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            //// Configure mock: AddAsync  use Callback to simulate ID generation
            mockUserRepository
                .Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Callback<User>(user => user.Id = 1)
                .Returns(Task.CompletedTask);

            //// Act  Execute the method being tested
            var result = await userService.CreateUserAsync(createUserDto);

            //// Assert - Verify the results
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@yahoo.com", result.Email);
            Assert.Equal("User", result.Role);

            // Verify repository was called once
            mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()),Times.Once
            );
        }
    }
}