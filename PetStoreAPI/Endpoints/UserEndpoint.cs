using Microsoft.AspNetCore.Authorization;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace PetStoreAPI.Endpoints;

public static class UserEndpoint
{
    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/Users");

        // Đăng nhập
        group.MapPost("/login", async ([FromBody] LoginDTO loginDTO, PetStoreDbContext db, HttpContext httpContext, ILogger<Program> logger) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email && u.Password == loginDTO.Password);
            if (user == null)
            {
                return Results.Unauthorized();
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

            return Results.Ok();
        });

        // Đăng xuất
        group.MapPost("/logout", async (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/"); // Redirect to the home page after logging out
        });

        return group;
    }
}
