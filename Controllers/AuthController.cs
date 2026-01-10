using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Interfaces;

namespace TaskManagerAPI.Controllers
{
    
    /// Handles user authentication operations including login and logout
  
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

       
        /// Initializes the authentication controller with user service dependency
       
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        
        /// Authenticates a user and creates a session cookie
      
      //Success message with user data, or unauthorized error</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Validate the login request
            if (request == null || string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest(new { message = "Username is required" });
            }

            // Find the user by username
            var user = await _userService.GetUserByUsernameAsync(request.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

          
            // If password verification fails, return unauthorized

            // Create user claims for the authentication cookie
            var userClaims = CreateUserClaims(user);

            // Build the claims identity with cookie authentication scheme
            var claimsIdentity = new ClaimsIdentity(
                userClaims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in the user with a persistent cookie that expires in 24 hours
            await SignInUserAsync(claimsPrincipal);

            return Ok(new
            {
                message = "Login successful",
                user
            });
        }

        #region Private Helper Methods

      
        ////// Creates a list of claims based on user information
      
        /// The authenticated user DTO
        /// List of claims containing user identity and role information
        private List<Claim> CreateUserClaims(UserDto user)
        {
            return new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };
        }

       
        /// Signs in the user with a persistent authentication cookie
      
        /// The claims principal containing user information
        private async Task SignInUserAsync(ClaimsPrincipal principal)
        {
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Cookie persists across browser sessions
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24), // Cookie expires after 24 hours
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, principal,authenticationProperties
            );
        }

        #endregion
    }
}