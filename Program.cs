using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net;
using TaskManagerAPI.Data;
using TaskManagerAPI.Repositories;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services;
using TaskManagerAPI.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);


#region Controllers and API Documentation

// Add MVC controllers for handling HTTP requests
builder.Services.AddControllers();

// Enable API endpoint exploration for Swagger
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Manager API",
        Version = "v1",
        Description = "A RESTful API for managing tasks and users with role-based access control",
        Contact = new OpenApiContact
        {
            Name = "Hamdi Yaseen",
            Email = "hamdi_yafa@live.com"
        }
    });
});

#endregion

#region Database Configuration

// Configure in-memory database for development and testing
// Note: Replace with a real database (SQL Server, PostgreSQL, etc.) for production
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TaskManagementDb"));

#endregion

#region Dependency Injection - Repositories

// Register repository implementations for data access layer
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

#endregion

#region Dependency Injection - Services

// Register service implementations for business logic layer
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();


#endregion

/////Cookie Authentication
// Configure cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Read configuration values or use sensible defaults
        options.LoginPath = builder.Configuration["Authentication:Cookie:LoginPath"] ?? "/api/auth/login";
        options.LogoutPath = builder.Configuration["Authentication:Cookie:LogoutPath"] ?? "/api/auth/logout";

        // Cookie expiration: default 24 hours (1440 minutes)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(
            int.Parse(builder.Configuration["Authentication:Cookie:ExpireTimeMinutes"] ?? "1440")
        );

        // Sliding expiration: cookie lifetime extends with each request
        options.SlidingExpiration = bool.Parse(
            builder.Configuration["Authentication:Cookie:SlidingExpiration"] ?? "true"
        );

        // Security settings
        options.Cookie.HttpOnly = true; // Prevent JavaScript access to cookies
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Require HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict; // Prevent CSRF attacks
    });

builder.Services.AddAuthorization();


#region CORS Configuration

// Configure Cross-Origin Resource Sharing for API access
// WARNING: "AllowAll" is permissive - restrict in production
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

#endregion


var app = builder.Build();

#region Database Initialization

// Seed the database with initial data on application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    // Ensure database is created and seeded
    context.Database.EnsureCreated();
}
#endregion


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#region Middleware Pipeline

// Configure HTTP request pipeline
app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseCors("AllowAll"); // Enable CORS (add before authentication)
app.UseAuthentication(); // Enable authentication
app.UseAuthorization(); // Enable authorization

// Map controller endpoints
app.MapControllers();

#endregion





// Start the application
app.Run();