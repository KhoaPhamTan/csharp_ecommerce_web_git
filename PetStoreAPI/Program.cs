using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Endpoints;
using PetStoreAPI.EndPoints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with SQLite
builder.Services.AddDbContext<PetStoreDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PetStore")));

// Add CORS to allow access from the UI (localhost:5135 or any other origin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:5135")  // Ensure this matches the UI origin
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

builder.Services.AddAuthorization(); // Add authorization services

var app = builder.Build();

// Run migrations automatically (if needed)
await app.MigrateDbAsync(); // Custom method to migrate the database

// Use CORS before authentication and authorization middlewares
app.UseCors("AllowSpecificOrigin"); // Ensure CORS is used before authentication and authorization

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization(); // Ensure authorization middleware is used

// Map endpoints
app.MapStoresEndPoints();
app.MapCartEndpoints();
app.MapRegistrationEndpoints();
app.MapPetEndpoints();
app.MapCategoryEndpoints(); // Ánh xạ các endpoints liên quan đến Category
app.MapUserEndpoints(); // Ánh xạ các endpoints liên quan đến người dùng

// Run the application
app.Run();
