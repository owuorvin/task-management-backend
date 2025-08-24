## Task Management API
.NET 8 REST API for task management with JWT authentication.
Quick Start
# Clone and run
git clone [https://github.com/owuorvin/task-management-backend]
cd SmoothStack
dotnet restore
dotnet run
API runs on https://localhost:7243. Swagger docs at /swagger.

## Features

JWT authentication (tokens expire in 60 minutes)
Task CRUD with drag-drop status updates
Role-based access (Admin/User)
In-memory database (zero config for development)

## Tech Stack

.NET 8.0
Entity Framework Core
JWT Bearer Auth
Swagger/OpenAPI
xUnit for testing

## Project Structure
backend/
├── SmoothStack/                 # Main API project
│   ├── Controllers/             
│   │   ├── AuthController.cs    # Login/Register endpoints
│   │   ├── TasksController.cs   # Task management
│   │   └── UsersController.cs   # User listing
│   ├── Models/                  
│   │   ├── User.cs             # User entity
│   │   ├── Tasks.cs            # Task entity (yes, plural, I know...)
│   │   └── DTOs/               # Data transfer objects
│   ├── Services/               
│   │   ├── AuthService.cs      # Authentication logic
│   │   ├── TaskService.cs      # Task business logic
│   │   └── PasswordHasher.cs   # Security stuff
│   ├── Helper/                 
│   │   ├── AppDbContext.cs     # EF Core context
│   ├── Middleware/             
│   │   └── ErrorHandlingMiddleware.cs  # Global error handling
│   └── Program.cs              # Entry point with all the setup
│
└── SmoothStack.Tests/          # Unit tests (yes, I actually wrote some!)
    └── AuthServiceTests.cs     # Auth service tests

Configuration
appsettings.json:
json{
  "UseInMemoryDatabase": true,
  "JwtSettings": {
    "SecretKey": "CHANGE_THIS_IN_PRODUCTION",
    "ExpirationMinutes": 60
  }
}
## API Endpoints
Auth

POST /api/auth/register - Create account
POST /api/auth/login - Get JWT token

Tasks (requires auth)

GET /api/tasks - List tasks (supports filtering)
POST /api/tasks - Create task
PUT /api/tasks/{id} - Update task
DELETE /api/tasks/{id} - Delete task

Users (requires auth)

GET /api/users - List users

## Test Accounts

Admin: admin@gmail.com / admin123
User: gare@gmail.com / user123

Running Tests
bashcd SmoothStack.Tests
dotnet test
Currently testing:

✅ User registration/login
<img width="1086" height="97" alt="image" src="https://github.com/user-attachments/assets/b4f1065a-6bac-40e2-b85d-ced5c49e0b5e" />


## Common Issues
HTTPS Certificate Error:
bashdotnet dev-certs https --trust
CORS Issues:
Add your frontend URL in Program.cs:
csharppolicy.WithOrigins("http://localhost:3000")
Port Already in Use:
Change port in launchSettings.json
Production Deployment

Change JWT secret key
Switch to SQL Server:

Set UseInMemoryDatabase: false
Update connection string
Run migrations: dotnet ef database update


Build: dotnet publish -c Release
