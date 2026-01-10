using TaskManagerAPI.Entities;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services.Interfaces;

namespace TaskManagerAPI.Services
{
    /// <summary>
    /// Service layer for managing user business logic.
    /// Handles user operations, validation, and password hashing.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

       
        /// Initializes the user service with required repository

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// Retrieves all users in the system

        /// returns List of all users as DTOs
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(user => MapToDto(user)).ToList();
        }


        /// Retrieves a specific user by their ID


        /// returns User DTO if found, null otherwise
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return MapToDto(user);
        }

      
        /// Retrieves a user by their username
     
      
        /// returns user DTO if found, null otherwise
   
        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }

            return MapToDto(user);
        }


        /// Creates a new user in the system


        ////// returns the created user as a DTO
        ////// exception ArgumentException Thrown when validation fails
        /// exception InvalidOperationException">Thrown when username already exists
        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            // Validate required fields
            ValidateUserCreation(dto);

            // Check if username already exists
            await ValidateUsernameAvailable(dto.Username!);

            // Hash the password for secure storage
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create the user entity
            var user = new User
            {
                Username = dto.Username!,
                Email = dto.Email!,
                PasswordHash = passwordHash,
                Role = Enum.Parse<Role>(dto.Role!),
                CreatedAt = DateTime.UtcNow
            };

            // Save to database
            await _userRepository.AddAsync(user);

            return MapToDto(user);
        }


        /// Updates an existing user's information


        /// Updated user data
        /// returns updated user DTO if found, null otherwise
        /// exception InvalidOperationException" Thrown when username already exists
        public async Task<UserDto?> UpdateUserAsync(int id, CreateUserDto dto)
        {
            // Find the existing user
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            // Update username if provided and different
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.Username)
            {
                  // Verify new username isn't already taken
                await ValidateUsernameAvailable(dto.Username);
                user.Username = dto.Username;
            }

            ///// Update email if provided
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                user.Email = dto.Email;
            }

            /// Update password if provided (hash it first!)
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            //// Update role if provided
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                user.Role = Enum.Parse<Role>(dto.Role);
            }

            ///// Save changes to database
            await _userRepository.UpdateAsync(user);

            return MapToDto(user);
        }

      
        /// Deletes a user from the system
      
   
        /// returns Always returns true (consider changing this to match actual deletion result)
        public async Task<bool> DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
            return true;
        }

        #region Private Helper Methods

        
        /// Validates user creation data
       
        private void ValidateUserCreation(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                throw new ArgumentException("Username is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("Email is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("Password is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Role))
            {
                throw new ArgumentException("Role is required");
            }
        }

      
        /// Validates that a username is not already taken
      
        private async Task ValidateUsernameAvailable(string username)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists");
            }
        }

      
        /// Maps a User entity to a UserDto (excludes sensitive data like password hash)
        
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

        #endregion
    }
}