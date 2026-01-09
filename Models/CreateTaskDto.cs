using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using TaskManagerAPI.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using TaskManagerAPI.Models;


namespace TaskManagerAPI.Models
{
    
    public class CreateTaskDto
    {
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public Entities.TaskStatus? Status { get; set; }

        public int? AssignedUserId { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
