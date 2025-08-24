using Microsoft.EntityFrameworkCore;
using SmoothStack.Helper;
using SmoothStack.Services.Auth;
using SmoothStack.Services.Task;

namespace SmoothStack
{
    public static class ExtensionManager
    {
        public static IServiceCollection AddServices(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            services.AddHttpClient();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<ITaskService, TaskService>();

            // Register DbContext 
            services.AddDbContext<AppDbContext>(options);

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
