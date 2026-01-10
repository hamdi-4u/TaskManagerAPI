using TaskManagerAPI.Entities;

namespace TaskManagerAPI.Repositories.Interfaces
{
    // Interface for task repository operations
    public interface ITaskRepository
    {

        // Gets all tasks from the database
        Task<IEnumerable<TaskItem>> GetAllAsync();

        // Retrieve a single task by its ID
        Task<TaskItem?> GetByIdAsync(int id);

        // Get tasks filtered by user - useful for dashboard view
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);

        // Add new task to DB
        Task<TaskItem> AddAsync(TaskItem task);

        // Update existing task (returns null if not found)
        Task<TaskItem?> UpdateAsync(TaskItem task);

        // Delete task by id, returns true if successful
        Task<bool> DeleteAsync(int id);  // Remove Task from database
    }
}