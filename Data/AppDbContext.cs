using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Entities;

namespace TaskManagerAPI.Data
{
   
    /// Database context for the Task Manager application.
    /// Manages Users, Tasks, and their relationships.
   
    public class AppDbContext : DbContext
    {
       
        /// Initializes a new instance of the AppDbContext with the specified options
       
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

       
        /// Collection of all users in the system
      
        public DbSet<User> Users => Set<User>();

        
        /// Collection of all tasks in the system
       
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

      
        /// Configures entity relationships and seeds initial data
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            ConfigureRelationships(modelBuilder);

            // Seed initial data
            SeedUsers(modelBuilder);
            SeedTasks(modelBuilder);
        }

       
        /// Configures the relationship between Users and Tasks
       
        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // One User can have many Tasks
            // When a User is deleted, all their Tasks are also deleted (Cascade)
            modelBuilder.Entity<TaskItem>().HasOne(task => task.AssignedUser).WithMany(user => user.Tasks)
                .HasForeignKey(task => task.AssignedUserId).OnDelete(DeleteBehavior.Cascade);
        }

       
        /// Seeds initial users for development and testing
       
        private void SeedUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Email = "admin@taskmanager.com",
                    Role = Role.Admin,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2,
                    Username = "user",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Email = "user@taskmanager.com",
                    Role = Role.User,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }

       
        /// Seeds initial tasks for development and testing
       
        private void SeedTasks(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem
                {
                    Id = 1,
                    Title = "Setup project",
                    Description = "Initialize the project structure and dependencies",
                    AssignedUserId = 2,
                    Status = Entities.TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Create API endpoints",
                    Description = "Implement RESTful API endpoints for task management",
                    AssignedUserId = 2,
                    Status = Entities.TaskStatus.InProgress,
                    CreatedAt = DateTime.UtcNow
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Add Swagger documentation",
                    Description = "Integrate Swagger/OpenAPI for API documentation",
                    AssignedUserId = 2,
                    Status = Entities.TaskStatus.Completed,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}