using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Entities;
using TaskManagerAPI.Repositories.Interfaces;

namespace TaskManagerAPI.Repositories
{
    /// <summary>
    /// Repository for managing user data access operations.
    /// Handles CRUD operations and queries for users in the database.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

       
        /// Initializes the user repository with database context
        
        
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

       
        /// Retrieves all users from the database
       
        /// returns Complete list of all users
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

      
        /// Retrieves a specific user by their ID, including their assigned tasks
       
        /// <param name="id">The user ID to retrieve</param>
        /// returns The user with their tasks if found, null otherwise
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.Include(user => user.Tasks).FirstOrDefaultAsync(user => user.Id == id);
        }

        
        /// Retrieves a user by their username
        
        /// <param name="username">The username to search for</param>
        /// returns the user if found, null otherwise
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
        }

       
        /// Adds a new user to the database
      
        /// <param name="user">The user entity to add</param>
        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

   
        /// Updates an existing user in the database
       
        /// <param name="user">The user entity with updated values</param>
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

      
        /// Deletes a user from the database
        public async Task DeleteAsync(int id)
        {
            var userToDelete = await GetByIdAsync(id);

            if (userToDelete != null)
            {
                _context.Users.Remove(userToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}