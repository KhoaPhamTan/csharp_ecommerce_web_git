using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;
using PetStoreAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace PetStoreAPI.Endpoints
{
    public class RegistrationLogger { }

    public static class RegistrationEndpoint
    {
        public static RouteGroupBuilder MapRegistrationEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/registration");

            group.MapPost("/", async ([FromBody] UserRegistrationDTO userDto, PetStoreDbContext db, ILogger<RegistrationLogger> logger) =>
            {
                logger.LogInformation("Received registration request for user: {Username}", userDto.Username);

                if (db.Users == null || await db.Users.AnyAsync(u => u.Username == userDto.Username))
                {
                    logger.LogWarning("Username already exists: {Username}", userDto.Username);
                    return Results.BadRequest("Username already exists.");
                }

                var user = new UserEntity
                {
                    Username = userDto.Username,
                    Password = HashPassword(userDto.Password),
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    Address = userDto.Address,
                    Role = UserRole.Customer // Default role
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();
                logger.LogInformation("User registered successfully: {Username}", userDto.Username);
                return Results.Created($"/registration/{user.Id}", user);
            });

            return group;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
