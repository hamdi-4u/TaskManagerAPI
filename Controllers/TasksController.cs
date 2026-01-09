using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManagerAPI.Models; 
using TaskManagerAPI.Services; 
using TaskManagerAPI.Services.Interfaces;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }


        ////// admin only can create task details and creates a new task. 
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto dto)
        {
            try
            {
                var task = await _taskService.CreateTaskAsync(dto);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

       
        /// Here weretrieves tasks. admins see all tasks, users see only their assigned tasks.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            IEnumerable<TaskDto> tasks;

            if (isAdmin)
            {
                tasks = await _taskService.GetAllTasksAsync();
            }
            else
            {
                tasks = await _taskService.GetTasksByUserIdAsync(currentUserId);
            }

            return Ok(tasks);
        }

        
        ///// Here we retrieves a specific task by ID , user or admin
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTaskById(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound(new { message = "task by Id not found" });
            }

            if (!isAdmin && task.AssignedUserId != currentUserId)
            {
                return Forbid();
            }

            return Ok(task);
        }

       
        ////// for Updates a task. admins can update all fields of any task. 
        //// Role for users can only update the status of tasks assigned to them.
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] CreateTaskDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";

            try
            {
                var updatedTask = await _taskService.UpdateTaskAsync(id, dto, currentUserId, currentUserRole);

                if (updatedTask == null)
                {
                    return NotFound(new { message = "An update task not found" });
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



        /////-- admin only can delete a task from the system. 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);

            if (!result)
            {
                return NotFound(new { message = "can'not delete a task not exist" });
            }

            return NoContent();
        }

        
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }
    }
}
