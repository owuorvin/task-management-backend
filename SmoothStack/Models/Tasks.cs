using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmoothStack.Models
{
    public class Tasks
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public TasksStatus Status { get; set; } = TasksStatus.TODO;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskPriority Priority { get; set; } = TaskPriority.MEDIUM;

        public int? AssigneeId { get; set; }
        public User? Assignee { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TasksStatus
    {
        TODO,
        IN_PROGRESS,
        DONE
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskPriority
    {
        LOW,
        MEDIUM,
        HIGH,
        URGENT
    }
}
