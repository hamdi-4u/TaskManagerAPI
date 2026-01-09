using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> AuthenticateAsync(string username, string password);
    }
}