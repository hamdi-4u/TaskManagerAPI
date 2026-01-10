using Moq;
using TaskManagerAPI.Entities;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services;
using Xunit;

namespace TaskManagerAPI.Tests
{
   
    /// Unit tests for the TaskService.
    /// Tests task creation, validation, and business logic.
   
    public class TaskServiceTests
    {
       
        /// Verifies that a task is created successfully with valid data
       
        [Fact]
        public async Task CreateTaskAsync_WithValidData_ReturnsTaskDto()
        {
            //// Arrange - Set up test data and dependencies
            var mockTaskRepository = new Mock<ITaskRepository>();
            var mockUserRepository = new Mock<IUserRepository>();
            var taskService = new TaskService(mockTaskRepository.Object, mockUserRepository.Object);

            var createTaskDto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Test Description",
                AssignedUserId = 2,
                Status = Entities.TaskStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            var assignedUser = new User
            {
                Id = 2,
                Username = "testuser",
                Email = "test@gmail.com",
                PasswordHash = "hashedpassword",
                Role = Role.User,
                CreatedAt = DateTime.UtcNow
            };

            var createdTask = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                Description = "Test Description",
                Status = Entities.TaskStatus.Pending,
                AssignedUserId = 2,
                AssignedUser = assignedUser,
                DueDate = createTaskDto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            // Configure mock: User exists
            mockUserRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(assignedUser);

            // Configure mock: Task is added successfully
            mockTaskRepository.Setup(repo => repo.AddAsync(It.IsAny<TaskItem>())).ReturnsAsync(createdTask);

            // Configure mock: GetByIdAsync returns task with user
            mockTaskRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(createdTask);

            // Act - Execute the method being tested
            var result = await taskService.CreateTaskAsync(createTaskDto);

            // Assert - Verify the results
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Task", result.Title);
            Assert.Equal("Test Description", result.Description);
            Assert.Equal(Entities.TaskStatus.Pending, result.TaskStatus);
            Assert.Equal(2, result.AssignedUserId);
            Assert.Equal("testuser", result.AssignedUserName);
            Assert.Equal("test@example.com", result.AssignedUserEmail);

            // Verify repository methods were called correctly
            mockUserRepository.Verify(repo => repo.GetByIdAsync(2),Times.Once);

            mockTaskRepository.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()),Times.Once);
        }

       
        /// Verifies that an exception is thrown when trying to assign a task to a non-existent user
       
        [Fact]
        public async Task CreateTaskAsync_WithNonExistentUser_ThrowsException()
        {
            // Arrange - Set up test data and dependencies
            var mockTaskRepository = new Mock<ITaskRepository>();
            var mockUserRepository = new Mock<IUserRepository>();
            var taskService = new TaskService(mockTaskRepository.Object, mockUserRepository.Object);

            var createTaskDto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Test Description",
                AssignedUserId = 999 // Non-existent user ID
            };

            // Configure mock: User does not exist
            mockUserRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((User?)null);

            // Act & Assert - Verify exception is thrown
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => taskService.CreateTaskAsync(createTaskDto)
            );

            Assert.Equal("Assigned user does not exist", exception.Message);

            // Verify AddAsync was never called since validation failed
            mockTaskRepository.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()),Times.Never);
        }
    }
}