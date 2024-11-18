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

// Add authentication and authorization services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";               // Đường dẫn trang đăng nhập
        options.LogoutPath = "/Account/Logout";             // Đường dẫn trang đăng xuất
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn trang từ chối quyền truy cập
    });
builder.Services.AddAuthorization(); // Add authorization services

// Add CORS to allow access from the UI (localhost:5135 or any other origin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("http://localhost:5135")  // Đảm bảo cho phép yêu cầu từ UI (localhost:5135)
                   .AllowAnyMethod()  // Cho phép tất cả các phương thức (GET, POST, PUT, DELETE, etc.)
                   .AllowAnyHeader()  // Cho phép tất cả các header
                   .AllowCredentials(); // Cho phép gửi cookie nếu cần
        });
});

var app = builder.Build();

// Run migrations automatically (if needed)
await app.MigrateDbAsync(); // Custom method to migrate the database

// Use CORS before authentication and authorization middlewares
app.UseCors("AllowAll"); // Đảm bảo đặt CORS trước middleware Authentication và Authorization

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization(); // Đảm bảo authorization middleware được sử dụng

// Map endpoints
app.MapStoresEndPoints();
app.MapCartEndpoints();
app.MapRegistrationEndpoints();
app.MapPetEndpoints();
app.MapCategoryEndpoints(); // Ánh xạ các endpoints liên quan đến Category
app.MapUserEndpoints(); // Ánh xạ các endpoints liên quan đến người dùng

// Run the application
app.Run();
