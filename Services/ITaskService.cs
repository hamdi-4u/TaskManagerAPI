using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing task-related business logic.
    /// Provides methods for CRUD operations, task queries, and role-based updates.
    /// </summary>
    public interface ITaskService
    {
      
        /// Retrieves all tasks in the system
      
        /// Collection of all tasks as DTOs
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();


        /// Retrieves all tasks assigned to a specific user by user ID

        Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(int userId);

       
        /// Retrieves a specific task by its unique identifier
      
        Task<TaskDto?> GetTaskByIdAsync(int id);

     
        //// Creates a new task in the system
      
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto);

    
        /// Updates an existing task with role-based authorization.
       
        Task<TaskDto?> UpdateTaskAsync(int id, CreateTaskDto dto, int currentUserId, string currentUserRole);

     
        ///// Deletes a task from the system
        Task<bool> DeleteTaskAsync(int id);
    }
}