using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Interfaces;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _IuserService;

        public UsersController(IUserService userService)
        {
            _IuserService = userService;
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                var user = await _IuserService.CreateUserAsync(dto);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                
                if (ex.Message.Contains("already exists") || ex.Message.Contains("duplicate"))
                {
                    return Conflict(new { message = ex.Message });
                }

                return BadRequest(new { message = ex.Message });
            }
        }

        
      
       
        //// we list of all users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _IuserService.GetAllUsersAsync();
            return Ok(users);
        }


     

        //// Here we retrieves a specific user by ID. 
        ////..admins can view any user, regular users can only view themselves.
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

          
            if (!isAdmin && currentUserId != id)
            {
                return Forbid();
            }

            var user = await _IuserService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "a user by Id not found" });
            }

            return Ok(user);
        }


        ///// we do updates an existing user by admin.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] CreateUserDto dto)
        {
            try
            {
                var updatedUser = await _IuserService.UpdateUserAsync(id, dto);

                if (updatedUser == null)
                {
                    return NotFound(new { message = "an updating user not exist" });
                }

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
               
                if (ex.Message.Contains("already exists") || ex.Message.Contains("duplicate"))
                {
                    return Conflict(new { message = ex.Message });
                }

                return BadRequest(new { message = ex.Message });
            }
        }


        ///////// admin role only can delete a user from the system. .
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _IuserService.DeleteUserAsync(id);

            if (!result)
            {
                return NotFound(new { message = "a deleting user not exist" });
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