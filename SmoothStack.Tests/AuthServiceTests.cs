using Microsoft.EntityFrameworkCore;
using SmoothStack.Helper;
using SmoothStack.Models.DTOs;
using SmoothStack.Models;
using SmoothStack.Services.Auth;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace SmoothStack.Tests.Services
{
    public class AuthServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _passwordHasher = new PasswordHasher();

            var configuration = new ConfigurationBuilder()
              .AddInMemoryCollection(new Dictionary<string, string?>
              {
                    {"JwtSettings:SecretKey", "TestSecretKeyThatIsLongEnoughForTesting123456"},
                    {"JwtSettings:Issuer", "TestIssuer"},
                    {"JwtSettings:Audience", "TestAudience"},
                    {"JwtSettings:ExpirationMinutes", "60"}
              })
              .Build();

            _tokenGenerator = new JwtTokenGenerator(configuration);
            _authService = new AuthService(_context, _passwordHasher, _tokenGenerator);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_CreatesNewUser()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal("testuser", result.User.Username);
            Assert.Equal("test@example.com", result.User.Email);
            Assert.Equal("USER", result.User.Role);

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            Assert.NotNull(userInDb);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = _passwordHasher.HashPassword("password123"),
                Role = UserRole.USER
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal("testuser", result.User.Username);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
