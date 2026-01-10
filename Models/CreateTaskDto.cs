using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using TaskManagerAPI.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using TaskManagerAPI.Models;



namespace TaskManagerAPI.Models
{
   
    /// Data transfer object for creating or updating a task.
    /// Contains all the information needed to create a new task in the system.
  
    public class CreateTaskDto
    {
  
        /// The task's title or name
   
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        /// Detailed description of what the task involves
       
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

 
        ///// Current status of the task (Pending, InProgress, Completed)
   
        public Entities.TaskStatus? Status { get; set; }

       
        //// ID of the user this task is assigned to.
        /// Null if the task is unassigned.
   
        public int? AssignedUserId { get; set; }

        ///// Optional deadline for when the task should be completed
      
        public DateTime? DueDate { get; set; }
    }
}