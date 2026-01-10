using TaskManagerAPI.Entities;

namespace TaskManagerAPI.Models
{
    /// <summary>
    /// Data transfer object for task information in API responses.
    /// </summary>
    public class TaskDto
    {


        ///// Unique identifier for the task
        public int Id { get; set; }



        /// The task's title or name
        public string Title { get; set; } = string.Empty;



        /// Detailed description of the task
        public string Description { get; set; } = string.Empty;


        //// Current status of the task (Pending, InProgress, or Completed)

        public Entities.TaskStatus TaskStatus { get; set; }



        ///// Optional deadline for when the task should be completed
        public DateTime? DueDate { get; set; }



        /// Timestamp when the task was created (in UTC)
        public DateTime CreatedAt { get; set; }



        /// ID of the user this task is assigned to
        public int AssignedUserId { get; set; }



        /// Username of the assigned user (for display purposes)
        public string AssignedUserName { get; set; } = string.Empty;


        /// Email address of the assigned user (optional)
        public string? AssignedUserEmail { get; set; }
    }
}