using Moq;
using TaskManagerAPI.Entities;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services;
using Xunit;

namespace TaskManagerAPI.Tests
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task CreateTaskAsync_WithValidData_ReturnsTaskDto()
        {
            
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
                Email = "test@live.com",
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

            //// Mock User exists
            mockUserRepository
                .Setup(repo => repo.GetByIdAsync(2))
                .ReturnsAsync(assignedUser);

            mockTaskRepository
                .Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(createdTask);

            // GetByIdAsync returns task with user
            mockTaskRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(createdTask);

            ////Act
            var result = await taskService.CreateTaskAsync(createTaskDto);

            //////// Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Task", result.Title);
            Assert.Equal("Test Description", result.Description);
            Assert.Equal(Entities.TaskStatus.Pending, result.TaskStatus);
            Assert.Equal(2, result.AssignedUserId);
            Assert.Equal("testuser", result.AssignedUserName);
            Assert.Equal("test@gmail.com", result.AssignedUserEmail);

            //// validate repository methods were called
            mockUserRepository.Verify(
                repo => repo.GetByIdAsync(2),
                Times.Once
            );

            mockTaskRepository.Verify(
                repo => repo.AddAsync(It.IsAny<TaskItem>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateTaskAsync_WithNonExistentUser_ThrowsException()
        {
            // Arrange
            var mockTaskRepository = new Mock<ITaskRepository>();
            var mockUserRepository = new Mock<IUserRepository>();
            var taskService = new TaskService(mockTaskRepository.Object, mockUserRepository.Object);

            var createTaskDto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Test Description",
                AssignedUserId = 999 
            };

            // Mock- User does not exist
            mockUserRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((User?)null);

            //Act &Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => taskService.CreateTaskAsync(createTaskDto)
            );

            Assert.Equal("assigned user does not exist", exception.Message);

            ///// detect AddAsync was never called
            mockTaskRepository.Verify(
                repo => repo.AddAsync(It.IsAny<TaskItem>()),
                Times.Never
            );
        }
    }
}