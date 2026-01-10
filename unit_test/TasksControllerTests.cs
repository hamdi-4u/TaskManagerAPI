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
    /// <summary>
    /// Unit tests for the TasksController.
    /// Tests role-based access control and task retrieval functionality.
    /// </summary>
    public class TasksControllerTests
    {
      
        /// Verifies that admin users can retrieve all tasks in the system
       
        [Fact]
        public async Task GetTasks_AsAdmin_ReturnsAllTasks()
        {
            // Arrange - Set up test data and dependencies
            var mockTaskService = new Mock<ITaskService>();
            var controller = new TasksController(mockTaskService.Object);

            var allTasks = new List<TaskDto>
            {
                new TaskDto
                {
                    Id = 1,
                    Title = "Task 1",
                    Description = "Description 1",
                    TaskStatus = Entities.TaskStatus.Pending,
                    AssignedUserId = 2,
                    AssignedUserName = "user1",
                    CreatedAt = DateTime.UtcNow
                },
                new TaskDto
                {
                    Id = 2,
                    Title = "Task 2",
                    Description = "Description 2",
                    TaskStatus = Entities.TaskStatus.InProgress,
                    AssignedUserId = 3,
                    AssignedUserName = "user2",
                    CreatedAt = DateTime.UtcNow
                },
                new TaskDto
                {
                    Id = 3,
                    Title = "Task 3",
                    Description = "Description 3",
                    TaskStatus = Entities.TaskStatus.Completed,
                    AssignedUserId = 2,
                    AssignedUserName = "user1",
                    CreatedAt = DateTime.UtcNow
                }
            };

            //// Create mock admin user with claims
            var adminClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var adminIdentity = new ClaimsIdentity(adminClaims, "TestAuth");
            var adminPrincipal = new ClaimsPrincipal(adminIdentity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminPrincipal }
            };

            //// Configure mock to return all tasks for admin
            mockTaskService
                .Setup(service => service.GetAllTasksAsync())
                .ReturnsAsync(allTasks);

            ///// Act - Execute the method being tested
            var result = await controller.GetTasks();

            // Assert - Verify the results
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);
            Assert.Equal(3, returnedTasks.Count());

            // Verify GetAllTasksAsync was called for Admin
            mockTaskService.Verify(  service => service.GetAllTasksAsync(),Times.Once);

            // Verify GetTasksByUserIdAsync was NOT called
            mockTaskService.Verify(service => service.GetTasksByUserIdAsync(It.IsAny<int>()),Times.Never);
        }

       
        /// Verifies that regular users only see tasks assigned to them
       
        [Fact]
        public async Task GetTasks_AsRegularUser_ReturnsOnlyUserTasks()
        {
            // Arrange - Set up test data and dependencies
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

            //// Create mock regular user (ID = 2) with claims
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),
                new Claim(ClaimTypes.Name, "user"),
                new Claim(ClaimTypes.Role, "User")
            };
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuth");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal }
            };

            //// Configure mock to return only user's tasks
            mockTaskService.Setup(service => service.GetTasksByUserIdAsync(2)).ReturnsAsync(userTasks);

            //// Act - Execute the method being tested
            var result = await controller.GetTasks();

            /// Assert - Verify the results
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);
            Assert.Equal(2, returnedTasks.Count());
            Assert.All(returnedTasks, task => Assert.Equal(2, task.AssignedUserId));

            /// Verify GetTasksByUserIdAsync was called with correct user ID
            mockTaskService.Verify(service => service.GetTasksByUserIdAsync(2),Times.Once);

            // Verify GetAllTasksAsync was NOT called for regular users
            mockTaskService.Verify(service => service.GetAllTasksAsync(),Times.Never);
        }
    }
}