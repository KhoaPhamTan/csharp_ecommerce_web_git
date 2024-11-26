using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Endpoints;
using PetStoreAPI.EndPoints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configure DbContext with SQLite
builder.Services.AddDbContext<PetStoreDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PetStore")));

// Add CORS to allow access from the UI (localhost:5135 or any other origin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:5135", "http://localhost:3000")  // Ensure this matches the UI origin
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // Allow sending cookies
        });
});

// Add authentication and authorization services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";               // Đường dẫn trang đăng nhập
        options.LogoutPath = "/Account/Logout";             // Đường dẫn trang đăng xuất
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn trang từ chối quyền truy cập

        // Ensure cookies are sent with cross-origin requests
        options.Cookie.SameSite = SameSiteMode.None;  // Configure cookie for cross-origin
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Use cookie only with HTTPS
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

// Ensure the app serves static files from the "wwwroot" directory


// Run migrations automatically (if needed)
await app.MigrateDbAsync(); // Custom method to migrate the database

// Use CORS before authentication and authorization middlewares
app.UseCors("AllowSpecificOrigin"); // Ensure CORS is used before authentication and authorization

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization(); // Ensure authorization middleware is used

// Add logging to verify endpoints are being mapped
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Mapping endpoints...");

// Map endpoints
app.MapStoresEndPoints();
app.MapCartEndpoints();
app.MapRegistrationEndpoints();
app.MapPetEndpoints();
app.MapCategoryEndpoints(); // Ánh xạ các endpoints liên quan đến Category
app.MapUserEndpoints(); // Ánh xạ các endpoints liên quan đến người dùng
app.MapAdminEndpoints(); // Add this line to map admin endpoints

logger.LogInformation("Endpoints mapped successfully");

// Add a simple log message to verify logging is working
logger.LogInformation("Application started successfully");

// Run the application
app.Run();
