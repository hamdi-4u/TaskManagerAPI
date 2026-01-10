using TaskManagerAPI.Entities;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services.Interfaces;

namespace TaskManagerAPI.Services
{
    /// <summary>
    /// Service layer for managing task business logic.
    /// Handles task operations, validation, and authorization rules.
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

       
        /// Initializes the task service with required repositories
        
        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

     
        /// Retrieves all tasks in the system
       
        /// returns List of all tasks as DTOs
        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Select(task => MapToDto(task)).ToList();
        }

       
        /// Retrieves a specific task by its ID
      
        
        /// returns Task DTO if found, null otherwise
        public async Task<TaskDto?> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return null;
            }

            return MapToDto(task);
        }


        /// Retrieves all tasks assigned to a specific user


        /// returns List of tasks assigned to the user
        public async Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(int userId)
        {
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            return tasks.Select(task => MapToDto(task)).ToList();
        }


        /// Creates a new task in the system

        
        /// returns The created task as a DTO
        /// exception ArgumentException Thrown when validation fails
        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto)
        {
            //// Validate required fields
            ValidateTaskCreation(dto);

            //// Verify the assigned user exists
            await ValidateUserExists(dto.AssignedUserId!.Value);

            ///// Create the task entity
            var task = new TaskItem
            {
                Title = dto.Title!,
                Description = dto.Description ?? string.Empty,
                Status = dto.Status ?? Entities.TaskStatus.Pending,
                AssignedUserId = dto.AssignedUserId.Value,
                DueDate = dto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            // Save to database
            var createdTask = await _taskRepository.AddAsync(task);

            // Reload task with user information
            var taskWithUser = await _taskRepository.GetByIdAsync(createdTask.Id);

            return MapToDto(taskWithUser!);
        }

  
        /// Updates an existing task with role based authorization.
        /// Admins can update all fields,regular users can only update status of their own tasks.
      
        public async Task<TaskDto?> UpdateTaskAsync(int id, CreateTaskDto dto, int currentUserId, string currentUserRole)
        {
            //// Find the existing task
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return null;
            }

            ////// Apply updates based on user role
            if (currentUserRole == "Admin")
            {
                await ApplyAdminUpdates(task, dto);
            }
            else
            {
                ApplyUserUpdates(task, dto, currentUserId);
            }

            //// Save changes to database
            await _taskRepository.UpdateAsync(task);

            ////// Reload task with user information
            var taskWithUser = await _taskRepository.GetByIdAsync(id);

            return MapToDto(taskWithUser!);
        }

      
        /// Deletes a task from the system
      
       
        /// <returns>True if deleted successfully, false if task not found</returns>
        public async Task<bool> DeleteTaskAsync(int id)
        {
            return await _taskRepository.DeleteAsync(id);
        }

        #region Private Helper Methods

       
        /// Validates task creation data
       
        private void ValidateTaskCreation(CreateTaskDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException("Title is required");
            }

            if (dto.AssignedUserId == null || dto.AssignedUserId <= 0)
            {
                throw new ArgumentException("Assigned user is required");
            }
        }

       
        /// Validates that a user exists in the system
    
        private async Task ValidateUserExists(int userId)
        {
            var userExists = await _userRepository.GetByIdAsync(userId);
            if (userExists == null)
            {
                throw new ArgumentException("Assigned user does not exist");
            }
        }
     
        /// Applies admin level updates to a task (can update all fields)
      
        private async Task ApplyAdminUpdates(TaskItem task, CreateTaskDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                task.Title = dto.Title;
            }

            if (dto.Description != null)
            {
                task.Description = dto.Description;
            }

            if (dto.Status != null)
            {
                task.Status = dto.Status.Value;
            }

            if (dto.DueDate != null)
            {
                task.DueDate = dto.DueDate;
            }

            if (dto.AssignedUserId != null && dto.AssignedUserId > 0)
            {
                // Verify the new assigned user exists
                await ValidateUserExists(dto.AssignedUserId.Value);
                task.AssignedUserId = dto.AssignedUserId.Value;
            }
        }

      
        //// Applies user level updates to a task (can only update status of own tasks)
     
        private void ApplyUserUpdates(TaskItem task, CreateTaskDto dto, int currentUserId)
        {
            //// Verify the task belongs to the current user
            if (task.AssignedUserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own tasks");
            }

            /// Regular users can only update the status
            if (dto.Status != null)
            {
                task.Status = dto.Status.Value;
            }
        }

      
        /// Maps a TaskItem entity to a TaskDto

        private TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                TaskStatus = task.Status,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                AssignedUserId = task.AssignedUserId,
                AssignedUserName = task.AssignedUser?.Username ?? string.Empty,
                AssignedUserEmail = task.AssignedUser?.Email
            };
        }

        #endregion
    }
}