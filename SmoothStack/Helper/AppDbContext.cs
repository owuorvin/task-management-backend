using Microsoft.EntityFrameworkCore;
using SmoothStack.Models;

namespace SmoothStack.Helper
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Task entity relationships
            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.Creator)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
    // Data seeder for initial data
    public static class DataSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                var passwordHasher = new Services.Auth.PasswordHasher();

                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@gmail.com",
                    PasswordHash = passwordHasher.HashPassword("admin123"),
                    Role = UserRole.ADMIN,
                    CreatedAt = DateTime.UtcNow
                };

                var normalUser = new User
                {
                    Username = "Gare",
                    Email = "gare@gmail.com",
                    PasswordHash = passwordHasher.HashPassword("user123"),
                    Role = UserRole.USER,
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.AddRange(adminUser, normalUser);
                context.SaveChanges();

                // Add sample tasks
                var tasks = new List<Tasks>
                {
                    new() {
                        Title = "Setup project repository",
                        Description = "Initialize Git repo and setup branch protection rules",
                        Status = TasksStatus.DONE,
                        Priority = TaskPriority.HIGH,
                        CreatorId = adminUser.Id,
                        AssigneeId = adminUser.Id
                    },
                    new() {
                        Title = "Design database schema",
                        Description = "Create ERD and define relationships between entities",
                        Status = TasksStatus.IN_PROGRESS,
                        Priority = TaskPriority.HIGH,
                        CreatorId = adminUser.Id,
                        AssigneeId = normalUser.Id
                    },
                    new() {
                        Title = "Implement authentication",
                        Description = "Add JWT based auth with refresh tokens",
                        Status = TasksStatus.IN_PROGRESS,
                        Priority = TaskPriority.URGENT,
                        CreatorId = normalUser.Id,
                        AssigneeId = normalUser.Id
                    },
                    new() {
                        Title = "Write unit tests",
                        Description = "Add comprehensive test coverage for services",
                        Status = TasksStatus.TODO,
                        Priority = TaskPriority.MEDIUM,
                        CreatorId = adminUser.Id,
                        AssigneeId = null
                    },
                    new() {
                        Title = "Setup CI/CD pipeline",
                        Description = "Configure GitHub Actions for automated deployments",
                        Status = TasksStatus.TODO,
                        Priority = TaskPriority.LOW,
                        CreatorId = normalUser.Id,
                        AssigneeId = adminUser.Id
                    }
                };

                context.Tasks.AddRange(tasks);
                context.SaveChanges();
            }
        }
    }
}
