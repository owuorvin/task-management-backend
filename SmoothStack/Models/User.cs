using System.ComponentModel.DataAnnotations;

namespace SmoothStack.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.USER;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public List<Tasks> CreatedTasks { get; set; } = new();
        public List<Tasks> AssignedTasks { get; set; } = new();
    }
    public enum UserRole
    {
        USER,
        ADMIN
    }
}
