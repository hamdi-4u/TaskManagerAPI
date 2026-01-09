using TaskManagerAPI.Entities;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services.Interfaces;

namespace TaskManagerAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Select(task => MapToDto(task)).ToList();
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) return null;

            return MapToDto(task);
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(int userId)
        {
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            return tasks.Select(task => MapToDto(task)).ToList();
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto)
        {
            //// Validate fields
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("a title is required");

            if (dto.AssignedUserId == null || dto.AssignedUserId <= 0)
                throw new ArgumentException("an assigned user is required");

           
            var userExists = await _userRepository.GetByIdAsync(dto.AssignedUserId.Value);
            if (userExists == null)
                throw new ArgumentException("an assigned user does not exist");

            ////TaskItem entity
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description ?? string.Empty,
                Status = dto.Status ?? Entities.TaskStatus.Pending, 
                AssignedUserId = dto.AssignedUserId.Value,
                DueDate = dto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            ///////Save to database
            var createdTask = await _taskRepository.AddAsync(task);


            var taskWithUser = await _taskRepository.GetByIdAsync(createdTask.Id);

          
            return MapToDto(taskWithUser!);
        }

        public async Task<TaskDto?> UpdateTaskAsync(int id, CreateTaskDto dto, int currentUserId, string currentUserRole)
        {
           
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) return null;

            //// an authorization logic based on Role
            if (currentUserRole == "Admin")
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
            
                    var userExists = await _userRepository.GetByIdAsync(dto.AssignedUserId.Value);
                    if (userExists == null)
                        throw new ArgumentException("an assigned user is not exist");

                    task.AssignedUserId = dto.AssignedUserId.Value;
                }
            }
            else //// regular user
            {
                //// Check if task belongs to the current user
                if (task.AssignedUserId != currentUserId)
                {
                    throw new UnauthorizedAccessException("you can only update your own tasks");
                }

                //// regular user can only update Status
                if (dto.Status != null)
                {
                    task.Status = dto.Status.Value;
                }

           
            }

            //// Save changes
            var updatedTask = await _taskRepository.UpdateAsync(task);


            var taskWithUser = await _taskRepository.GetByIdAsync(id);


            return MapToDto(taskWithUser!);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            return await _taskRepository.DeleteAsync(id);
        }

        ///// i added here helper method to map taskItem to taskDto
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
    }
}