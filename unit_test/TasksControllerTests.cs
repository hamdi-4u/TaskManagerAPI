using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TaskManagerAPI.Controllers;
using TaskManagerAPI.Entities;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Interfaces;
using Xunit;

namespace TaskManagerAPI.Tests
{
    public class TasksControllerTests
    {
        [Fact]
        public async Task GetTasks_AsAdmin_ReturnsAllTasks()
        {
            //// arrange
            var mockTaskService = new Mock<ITaskService>();
            var controller = new TasksController(mockTaskService.Object);

            var allTasks = new List<TaskDto>
            {
                new TaskDto
                {
                    Id = 1,
                    Title = "task 1",
                    Description = "description 1",
                    TaskStatus = Entities.TaskStatus.Pending,
                    AssignedUserId = 2,
                    AssignedUserName = "user1",
                    CreatedAt = DateTime.UtcNow
                },
                new TaskDto
                {
                    Id = 2,
                    Title = "task 2",
                    Description = "description 2",
                    TaskStatus = Entities.TaskStatus.InProgress,
                    AssignedUserId = 3,
                    AssignedUserName = "user2",
                    CreatedAt = DateTime.UtcNow
                },
                new TaskDto
                {
                    Id = 3,
                    Title = "task 3",
                    Description = "description 3",
                    TaskStatus = Entities.TaskStatus.Completed,
                    AssignedUserId = 2,
                    AssignedUserName = "user1",
                    CreatedAt = DateTime.UtcNow
                }
            };

            ////mock admin user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            mockTaskService
                .Setup(service => service.GetAllTasksAsync())
                .ReturnsAsync(allTasks);

            //// Act
            var result = await controller.GetTasks();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);
            Assert.Equal(3, returnedTasks.Count());

            // check if GetAllTasksAsync was called for Admin
            mockTaskService.Verify(
                service => service.GetAllTasksAsync(),
                Times.Once
            );

            // Verify GetTasksByUserIdAsync was not called
            mockTaskService.Verify(
                service => service.GetTasksByUserIdAsync(It.IsAny<int>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GetTasks_AsRegularUser_ReturnsOnlyUserTasks()
        {
            /////Arrange
            var mockTaskService = new Mock<ITaskService>();
            var controller = new TasksController(mockTaskService.Object);

            var userTasks = new List<TaskDto>
            {
                new TaskDto
                {
                    Id = 1,
                    Title = "My Task 1",
                    Description = "Description 1",
                    TaskStatus = Entities.TaskStatus.Pending,
                    AssignedUserId = 2,
                    AssignedUserName = "user",
                    CreatedAt = DateTime.UtcNow
                },
                new TaskDto
                {
                    Id = 3,
                    Title = "My Task 2",
                    Description = "Description 3",
                    TaskStatus = Entities.TaskStatus.Completed,
                    AssignedUserId = 2,
                    AssignedUserName = "user",
                    CreatedAt = DateTime.UtcNow
                }
            };

            ///// Mock Regular user ID = 2
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),
                new Claim(ClaimTypes.Name, "user"),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            mockTaskService
                .Setup(service => service.GetTasksByUserIdAsync(2))
                .ReturnsAsync(userTasks);

            // Act
            var result = await controller.GetTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);
            Assert.Equal(2, returnedTasks.Count());
            Assert.All(returnedTasks, task => Assert.Equal(2, task.AssignedUserId));

            // detect if GetTasksByUserIdAsync was called with correct user ID
            mockTaskService.Verify(
                service => service.GetTasksByUserIdAsync(2),
                Times.Once
            );

            // validate GetAllTasksAsync was NOT called
            mockTaskService.Verify(
                service => service.GetAllTasksAsync(),
                Times.Never
            );
        }
    }
}