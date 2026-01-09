
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TaskManagerAPI.Entities; 


namespace TaskManagerAPI.Data
{

 
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

         


            ////relationships Users And Tasks
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedUser)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            ////Seed initial users in memory DB
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = Role.Admin,
                    Email = "admin@taskmanager.com",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2,
                    Username = "user",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Role = Role.User,
                    Email = "user@taskmanager.com",
                    CreatedAt = DateTime.UtcNow
                }
            );

            ///Seed tasks
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem
                {
                    Id = 1,
                    Title = "Setup project",
                    Description = "Initial setup",
                    AssignedUserId = 2,
                    Status = Entities.TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Create endpoints",
                    Description = "API creation",
                    AssignedUserId = 2,
                    Status = Entities.TaskStatus.InProgress,
                    CreatedAt = DateTime.UtcNow
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Add Swagger",
                    Description = "Add docs",
                    AssignedUserId = 2,
                    Status = Entities.TaskStatus.Completed,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }


}
