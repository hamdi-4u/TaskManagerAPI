using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagerAPI.Controllers;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Interfaces;
using Xunit;

namespace TaskManagerAPI.Tests
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetAllUsers_ReturnsOkResult_WithListOfUsers()
        {
           
            var mockService = new Mock<IUserService>();
            var controller = new UsersController(mockService.Object);

            var users = new List<UserDto>
            {
                new UserDto
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@hotmail.com",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                },
                new UserDto
                {
                    Id = 2,
                    Username = "user",
                    Email = "user@yahoo.com",
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                }
            };

            mockService
                .Setup(service => service.GetAllUsersAsync())
                .ReturnsAsync(users);

            ///// Act
            var result = await controller.GetAllUsers();

            /////Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count());

            ///// Verify service was called once
            mockService.Verify(
                service => service.GetAllUsersAsync(),
                Times.Once
            );
        }
    }
}