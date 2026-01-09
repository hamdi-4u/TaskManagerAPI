using TaskManagerAPI.Data;
using TaskManagerAPI.Entities;
using TaskManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TaskManagerAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync() =>
            await _context.Tasks.Include(t => t.AssignedUser).ToListAsync();

        public async Task<TaskItem?> GetByIdAsync(int id) =>
            await _context.Tasks.Include(t => t.AssignedUser).FirstOrDefaultAsync(t => t.Id == id);

        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem?> UpdateAsync(TaskItem task)
        {
            var existingTask = await _context.Tasks.FindAsync(task.Id);
            if (existingTask == null) return null;

            _context.Entry(existingTask).CurrentValues.SetValues(task);
            await _context.SaveChangesAsync();
            return existingTask;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId) =>
        await _context.Tasks
        .Include(t => t.AssignedUser)
        .Where(t => t.AssignedUserId == userId)
        .ToListAsync();

    }
}