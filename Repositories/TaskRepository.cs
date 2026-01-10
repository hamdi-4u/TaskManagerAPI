using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Entities;
using TaskManagerAPI.Repositories.Interfaces;

namespace TaskManagerAPI.Repositories
{
    
    /// Repository for managing task data access operations.
    ////// Handles CRUD operations and queries for tasks in the database.
   
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

      
        ////// Initializes the task repository with database context
      
        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

       
        //// Retrieves all tasks from the database,including assigned user information
       
        ////// Complete list of all tasks with their assigned users
        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(task => task.AssignedUser).ToListAsync();
        }

        
        /// Retrieves a specific task by its ID,including assigned user information
       
        ///// The task if found, null otherwise
        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(task => task.AssignedUser).FirstOrDefaultAsync(task => task.Id == id);
        }

        /// Adds a new task to the database
      
        /// The task entity to add
        /// The added task with generated ID
        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Reload the task with related data
            await _context.Entry(task)
                .Reference(task => task.AssignedUser)
                .LoadAsync();

            return task;
        }

       
        /// Updates an existing task in the database
       
        ///The task entity with updated values
        ///The updated task if found, null if task doesn't exist
        public async Task<TaskItem?> UpdateAsync(TaskItem task)
        {
            var existingTask = await _context.Tasks.FindAsync(task.Id);

            if (existingTask == null)
            {
                return null;
            }

            // Update all properties from the new task to the existing task
            _context.Entry(existingTask).CurrentValues.SetValues(task);
            await _context.SaveChangesAsync();

            // Reload related data
            await _context.Entry(existingTask).Reference(task => task.AssignedUser)
                .LoadAsync();

            return existingTask;
        }

       
        /// Deletes a task from the database
        
        /// True if deleted successfully, false if task doesn't exist
        public async Task<bool> DeleteAsync(int id)
        {
            var taskToDelete = await _context.Tasks.FindAsync(id);

            if (taskToDelete == null)
            {
                return false;
            }

            _context.Tasks.Remove(taskToDelete);
            await _context.SaveChangesAsync();

            return true;
        }

        
        /// Retrieves all tasks assigned to a specific user
        
        /// 
        /// returns List of tasks assigned to the specified user
        public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.Tasks.Include(task => task.AssignedUser)
                .Where(task => task.AssignedUserId == userId).ToListAsync();
        }
    }
}