using Microsoft.AspNetCore.Authorization;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace PetStoreAPI.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users");

        // POST method for login
        group.MapPost("/login", async (LoginDTO loginDto, HttpContext httpContext, PetStoreDbContext dbContext, ILogger<Program> logger) =>
        {
            logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            // Kiểm tra dữ liệu gửi lên có hợp lệ không
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                logger.LogWarning("Email or password is missing.");
                return Results.BadRequest("Email or password is missing.");
            }

            // Kiểm tra tài khoản trong database
            var user = dbContext.Users != null ? await dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email) : null;
            if (user == null)
            {
                logger.LogWarning("Invalid login attempt for email: {Email}", loginDto.Email);
                return Results.BadRequest("Invalid login attempt");
            }

            // Xác minh mật khẩu (so sánh với mật khẩu mã hóa trong cơ sở dữ liệu)
            using var sha256 = SHA256.Create();
            var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)));

            // So sánh mật khẩu đã mã hóa (hash) với mật khẩu trong cơ sở dữ liệu
            if (user.Password != passwordHash)
            {
                logger.LogWarning("Invalid login attempt for email: {Email}", loginDto.Email);
                return Results.BadRequest("Invalid login attempt");
            }

            logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);

            // Tạo danh sách Claims cho người dùng
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // Lưu UserId
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),  // Lưu Username
                new Claim(ClaimTypes.Email, user.Email),  // Lưu Email
                new Claim(ClaimTypes.Role, user.Role.ToString())  // Cung cấp quyền cho user (Customer, Admin)
            };

            var userIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(userIdentity);

            // Đăng nhập người dùng
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Results.Ok("Login successful");
        });

        // POST method for logout
        group.MapPost("/logout", async (HttpContext httpContext, ILogger<Program> logger) =>
        {
            logger.LogInformation("Logout attempt");

            // Đăng xuất người dùng
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            logger.LogInformation("User logged out successfully");

            // Redirect to home page after logout
            return Results.Redirect("/");
        });

        // Route dành cho Admin
        group.MapGet("/admin", [Authorize(Roles = "Admin")] (ILogger<Program> logger) =>
        {
            logger.LogInformation("Admin endpoint accessed.");
            return Results.Ok("Welcome Admin!");
        });

        // Route dành cho Customer
        group.MapGet("/customer", [Authorize(Roles = "Customer")] (ILogger<Program> logger) =>
        {
            logger.LogInformation("Customer endpoint accessed.");
            return Results.Ok("Welcome Customer!");
        });

        return group;
    }
}
