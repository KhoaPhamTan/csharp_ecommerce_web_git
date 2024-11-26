using Microsoft.AspNetCore.Authorization;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace PetStoreAPI.Endpoints;

public static class UserEndpoint
{
    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/Users");

        // Đăng nhập
        group.MapPost("/login", async ([FromBody] LoginDTO loginDTO, PetStoreDbContext db, HttpContext httpContext, ILogger<Program> logger) =>
        {
            logger.LogInformation("Login attempt for email: {Email}", loginDTO.Email);

            // Hash the input password
            var hashedPassword = HashPassword(loginDTO.Password);

            // Check if the user exists
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (user == null)
            {
                logger.LogWarning("Login failed: User not found for email: {Email}", loginDTO.Email);
                return Results.Unauthorized();
            }

            // Log the user's role from the database
            logger.LogInformation("User found: {Email}, Role: {Role}", user.Email, user.Role.ToString());

            // Check if the password is correct
            if (user.Password != hashedPassword)
            {
                logger.LogWarning("Login failed: Incorrect password for email: {Email}", loginDTO.Email);
                return Results.Unauthorized();
            }

            // Check if the user has the admin role (case-insensitive)
            if (!user.Role.ToString().Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Login failed: User does not have admin role for email: {Email}", loginDTO.Email);
                return Results.Forbid();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Ensure userId is set as string
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Add role claim
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = loginDTO.RememberMe // Set the persistence of the authentication session based on RememberMe
            };

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Log the claims for debugging
            logger.LogInformation("User logged in with the following claims:");
            foreach (var claim in claims)
            {
                logger.LogInformation($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            // Return a token if needed
            var token = GenerateToken(user);
            return Results.Ok(new { token, role = user.Role.ToString() }); // Ensure role is included in the response
        });

        // Get list of users
        group.MapGet("/", async (PetStoreDbContext db) =>
        {
            var users = await db.Users
                .Select(u => new UserDTO(u.Id, u.Username, u.FullName, u.Email, u.Address, u.Role.ToString(), u.Password))
                .ToListAsync();
            return Results.Ok(users);
        });

        // Đăng xuất
        group.MapPost("/logout", async (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/"); // Redirect to the home page after logging out
        });

        group.MapPut("/{id}", async (int id, [FromBody] UserDTO updatedUser, PetStoreDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return Results.NotFound();
            }

            user.Username = updatedUser.Username;
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                user.Password = HashPassword(updatedUser.Password);
            }
            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.Address = updatedUser.Address;

            await db.SaveChangesAsync();
            return Results.Ok(new UserDTO(user.Id, user.Username, user.FullName, user.Email, user.Address, user.Role.ToString(), user.Password));
        });

        return group;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static string GenerateToken(PetStoreAPI.Entities.UserEntity user)
    {
        // Implement token generation logic here
        // For example, using JWT
        return "generated_token";
    }

    private static string GenerateToken()
    {
        // Implement token generation logic here
        // For example, using JWT
        return "generated_token";
    }
}
