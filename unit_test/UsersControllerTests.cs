using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagerAPI.Controllers;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Interfaces;
using Xunit;

namespace TaskManagerAPI.Tests
{
    /// <summary>
    /// Unit tests for the UsersController.
    /// Tests user retrieval and controller response behavior.
    /// </summary>
    public class UsersControllerTests
    {
      
        /// Verifies that GetAllUsers returns a successful response with a list of users
      
        [Fact]
        public async Task GetAllUsers_ReturnsOkResult_WithListOfUsers()
        {
            //// Arrange - Set up test data and dependencies
            var mockUserService = new Mock<IUserService>();
            var controller = new UsersController(mockUserService.Object);

            var users = new List<UserDto>
            {
                new UserDto
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@live.com",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                },
                new UserDto
                {
                    Id = 2,
                    Username = "user",
                    Email = "user@hotmail.com",
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                }
            };

            //// Configure mock to return test users
            mockUserService.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(users);

            //// Act - Execute the method being tested
            var result = await controller.GetAllUsers();

            // Assert - Verify the results
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count());

            // Verify service was called once
            mockUserService.Verify(service => service.GetAllUsersAsync(),Times.Once);
        }
    }
}