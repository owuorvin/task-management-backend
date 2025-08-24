using System.ComponentModel.DataAnnotations;

namespace SmoothStack.Models.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public TaskPriority Priority { get; set; } = TaskPriority.MEDIUM;

        public int? AssigneeId { get; set; }
    }

    public class UpdateTaskDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public TasksStatus? Status { get; set; }

        public TaskPriority? Priority { get; set; }

        public int? AssigneeId { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public UserDto? Assignee { get; set; }
        public UserDto Creator { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
