

using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Interfaces
{
    public interface IUserService
    {
       
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto?> UpdateUserAsync(int id, CreateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
    }
}