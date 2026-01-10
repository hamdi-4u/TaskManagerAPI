using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Interfaces;

namespace TaskManagerAPI.Controllers
{
    
    /// Manages task operations including creation, retrieval, updates, and deletion.
    ///// Implements role based access control for admins and regular users.
   
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;


        /// Initializes the tasks controller with task service dependency
      
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }


        /// Creates a new task in the system Admin only
       
        /// Task details including title, description, and assignment
        ////// The created task with generated ID
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto dto)
        {
            try
            {
                var createdTask = await _taskService.CreateTaskAsync(dto);
                return CreatedAtAction(
                    nameof(GetTaskById),
                    new { id = createdTask.Id },
                    createdTask
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

      
        /// Retrieves tasks based on user role.
        /// Admins see all tasks, regular users see only their assigned tasks.
      
        ///// List of tasks filtered by user permissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            IEnumerable<TaskDto> tasks;

            if (isAdmin)
            {
                //// Admins can view all tasks in the system
                tasks = await _taskService.GetAllTasksAsync();
            }
            else
            {
                /// Regular users only see tasks assigned to them
                tasks = await _taskService.GetTasksByUserIdAsync(currentUserId);
            }

            return Ok(tasks);
        }

        /// Users can only view tasks assigned to them, admins can view any task.
  
        
        /// Task details if found and user has permission
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTaskById(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            var task = await _taskService.GetTaskByIdAsync(id);

            // Check if task exists
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            // Verify user has permission to view this task
            if (!isAdmin && task.AssignedUserId != currentUserId)
            {
                return Forbid();
            }

            return Ok(task);
        }

     
        /// Updates an existing task.
        /// Admins can update all fields of any task.
        /// Regular users can only update the status of tasks assigned to them.
        /// Updated task information
        
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] CreateTaskDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

            try
            {
                var updatedTask = await _taskService.UpdateTaskAsync(
                    id,
                    dto,
                    currentUserId,
                    currentUserRole
                );

                if (updatedTask == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                return Ok(updatedTask);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        ///// Deletes a task from the system (Admin only)
  

        /// No content if successful, not found if task doesn't exist
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var wasDeleted = await _taskService.DeleteTaskAsync(id);

            if (!wasDeleted)
            {
                return NotFound(new { message = "Task not found" });
            }

            return NoContent();
        }

        #region Private Helper Methods

        /// Extracts the current user's ID from the authentication claims
       
        /// The authenticated user's ID
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        /// Checks if the current user has Admin role
         
        /// True if user is an admin, false otherwise
        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        #endregion
    }
}