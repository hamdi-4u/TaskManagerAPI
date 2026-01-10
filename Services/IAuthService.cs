using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing authentication operations.
    /// Handles user login and credential verification.
    /// </summary>
    public interface IAuthService
    {
       
        ///// Authenticates a user with their username and password
        Task<LoginResponse?> AuthenticateAsync(string username, string password);
    }
}