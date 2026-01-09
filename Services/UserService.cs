using TaskManagerAPI.Entities;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services.Interfaces;

namespace TaskManagerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(user => MapToDto(user)).ToList();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return MapToDto(user);
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            return MapToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            /////Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("username is required");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("email is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("password is required");

            if (string.IsNullOrWhiteSpace(dto.Role))
                throw new ArgumentException("role is required");

           
            var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
                throw new InvalidOperationException("username already exists");

            //// hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            //// create User entity
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = Enum.Parse<Role>(dto.Role), 
                CreatedAt = DateTime.UtcNow
            };

           
            await _userRepository.AddAsync(user);

        
            return MapToDto(user);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, CreateUserDto dto)
        {
            
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            //// Update username if provided and different
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.Username)
            {
                //// check if new username is already taken
                var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
                if (existingUser != null)
                    throw new InvalidOperationException("username already exists");

                user.Username = dto.Username;
            }

            ////update email
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                user.Email = dto.Email;
            }

            //// update password if provided hash it!.
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            /// update role if provided
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                user.Role = Enum.Parse<Role>(dto.Role);
            }

            await _userRepository.UpdateAsync(user);

         
            return MapToDto(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
            return true;
        }

        // helper method to map user entity to userDto
        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(), 
                CreatedAt = user.CreatedAt
             
            };
        }
    }
}