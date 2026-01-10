using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Interfaces;

namespace TaskManagerAPI.Controllers
{
   
    /// Manages user operations including creation, retrieval, updates, and deletion.
    /// Implements role-based access control for admins and regular users.
   
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

   
        /// Initializes the users controller with user service dependency
       
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

       
        /// Creates a new user in the system (Admin only)
        
        /// User details including username, email, password, and role
        /// The created user with generated ID
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(dto);
                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = createdUser.Id },
                    createdUser
                );
            }
            catch (Exception ex)
            {
                // Handle duplicate username/email conflicts
                if (ex.Message.Contains("already exists") || ex.Message.Contains("duplicate"))
                {
                    return Conflict(new { message = ex.Message });
                }

                return BadRequest(new { message = ex.Message });
            }
        }

       
        /// Retrieves a list of all users in the system (Admin only)
   
        /// <returns>Complete list of all registered users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        
        /// Retrieves a specific user by their ID.
        /// Admins can view any user, regular users can only view their own profile.
        

        /// The user ID to retrieve
        /// User details if found and user has permission
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            //// Regular users can only view their own profile
            if (!isAdmin && currentUserId != id)
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        
        //// Updates an existing user's information (Admin only)
     
        ///// The updated user details
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] CreateUserDto dto)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, dto);

                if (updatedUser == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                // Handle duplicate username/email conflicts during update
                if (ex.Message.Contains("already exists") || ex.Message.Contains("duplicate"))
                {
                    return Conflict(new { message = ex.Message });
                }

                return BadRequest(new { message = ex.Message });
            }
        }

      
        ///// Deletes a user from the system (Admin only)
        
        //// No content if successful, not found if user doesn't exist
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var wasDeleted = await _userService.DeleteUserAsync(id);

            if (!wasDeleted)
            {
                return NotFound(new { message = "User not found" });
            }

            return NoContent();
        }

        #region Private Helper Methods

      
        /// Extracts the current user's ID from the authentication claims
     
        ///// The authenticated user's ID
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        //// Checks if the current user has Admin role
      
        ////// True if user is an admin, false otherwise
        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        #endregion
    }
}