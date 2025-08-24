using SmoothStack.Helper;
using SmoothStack.Models.DTOs;
using SmoothStack.Models;
using Microsoft.EntityFrameworkCore;

namespace SmoothStack.Services.Task
{
    public class TaskService: ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskDto>> GetTasksAsync(string? status, int? assigneeId)
        {
            var query = _context.Tasks
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TasksStatus>(status, out var taskStatus))
            {
                query = query.Where(t => t.Status == taskStatus);
            }

            if (assigneeId.HasValue)
            {
                query = query.Where(t => t.AssigneeId == assigneeId.Value);
            }

            var tasks = await query.OrderByDescending(t => t.UpdatedAt).ToListAsync();
            return tasks.Select(MapToTaskDto).ToList();
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.Id == id);

            return task != null ? MapToTaskDto(task) : null;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createDto, int creatorId)
        {
            var task = new Tasks
            {
                Title = createDto.Title,
                Description = createDto.Description,
                Priority = createDto.Priority,
                Status = TasksStatus.TODO,
                CreatorId = creatorId,
                AssigneeId = createDto.AssigneeId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(task)
                .Reference(t => t.Creator)
                .LoadAsync();

            if (task.AssigneeId.HasValue)
            {
                await _context.Entry(task)
                    .Reference(t => t.Assignee)
                    .LoadAsync();
            }

            return MapToTaskDto(task);
        }

        public async Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto updateDto, int userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return null;

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateDto.Title))
                task.Title = updateDto.Title;

            if (!string.IsNullOrEmpty(updateDto.Description))
                task.Description = updateDto.Description;

            if (updateDto.Status.HasValue)
                task.Status = updateDto.Status.Value;

            if (updateDto.Priority.HasValue)
                task.Priority = updateDto.Priority.Value;

            if (updateDto.AssigneeId.HasValue || updateDto.AssigneeId == null)
                task.AssigneeId = updateDto.AssigneeId;

            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload assignee if changed
            if (task.AssigneeId.HasValue)
            {
                await _context.Entry(task)
                    .Reference(t => t.Assignee)
                    .LoadAsync();
            }

            return MapToTaskDto(task);
        }

        public async Task<bool> DeleteTaskAsync(int id, int userId)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return false;

            // Only creator or admin can delete
            var user = await _context.Users.FindAsync(userId);
            if (task.CreatorId != userId && user?.Role != UserRole.ADMIN)
                return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return true;
        }

        private TaskDto MapToTaskDto(Tasks task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                Assignee = task.Assignee != null ? new UserDto
                {
                    Id = task.Assignee.Id,
                    Username = task.Assignee.Username,
                    Email = task.Assignee.Email,
                    Role = task.Assignee.Role.ToString()
                } : null,
                Creator = new UserDto
                {
                    Id = task.Creator.Id,
                    Username = task.Creator.Username,
                    Email = task.Creator.Email,
                    Role = task.Creator.Role.ToString()
                },
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}
