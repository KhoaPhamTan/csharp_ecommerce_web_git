using Microsoft.AspNetCore.Authorization;
using PetStoreAPI.Data;
using PetStoreAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PetStoreAPI.Endpoints;

public static class AdminEndpoint
{
    public static RouteGroupBuilder MapAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/Admin").RequireAuthorization("AdminPolicy");

        // Get all users
        group.MapGet("/users", async (PetStoreDbContext db, ILogger<Program> logger, HttpContext httpContext) =>
        {
            logger.LogInformation("Admin /users endpoint hit");
            logger.LogInformation($"Request Method: {httpContext.Request.Method}");
            logger.LogInformation($"Request Path: {httpContext.Request.Path}");
            logger.LogInformation($"Request Headers: {string.Join(", ", httpContext.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");

            if (db.Users == null)
            {
                logger.LogWarning("Users DbSet is null");
                return Results.NotFound("Users not found");
            }
            var users = await db.Users.ToListAsync();
            logger.LogInformation($"Number of users retrieved: {users.Count}");
            return Results.Ok(users);
        });

        // Update user role
        group.MapPut("/users/{id}/role", async (int id, [FromBody] UserRole newRole, PetStoreDbContext db, ILogger<Program> logger, HttpContext httpContext) =>
        {
            logger.LogInformation($"Admin /users/{id}/role endpoint hit");
            logger.LogInformation($"Request Method: {httpContext.Request.Method}");
            logger.LogInformation($"Request Path: {httpContext.Request.Path}");
            logger.LogInformation($"Request Headers: {string.Join(", ", httpContext.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");

            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                logger.LogWarning($"User with id {id} not found");
                return Results.NotFound();
            }

            user.Role = newRole;
            await db.SaveChangesAsync();
            logger.LogInformation($"User with id {id} role updated to {newRole}");
            return Results.Ok(user);
        });

        return group;
    }
}