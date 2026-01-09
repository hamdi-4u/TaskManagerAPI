

using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Interfaces
{
    public interface ITaskService
    {
       
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();
        Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(int userId);

        Task<TaskDto?> GetTaskByIdAsync(int id);

        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto);

        Task<TaskDto?> UpdateTaskAsync(int id, CreateTaskDto dto, int currentUserId, string currentUserRole);
        Task<bool> DeleteTaskAsync(int id);
    }
}