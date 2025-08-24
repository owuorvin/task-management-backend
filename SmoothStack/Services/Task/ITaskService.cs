using SmoothStack.Models.DTOs;

namespace SmoothStack.Services.Task
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetTasksAsync(string? status, int? assigneeId);
        Task<TaskDto?> GetTaskByIdAsync(int id);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createDto, int creatorId);
        Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto updateDto, int userId);
        Task<bool> DeleteTaskAsync(int id, int userId);
    }
}
