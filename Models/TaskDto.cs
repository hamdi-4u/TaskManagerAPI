 
using TaskManagerAPI.Entities;
namespace TaskManagerAPI.Models
{

    /// 
    //// this task data of transfer object for API responses
    public class TaskDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Entities.TaskStatus TaskStatus { get; set; }  

        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public int AssignedUserId { get; set; }

        public string AssignedUserName { get; set; } = string.Empty;

        public string? AssignedUserEmail { get; set; }
    }
}