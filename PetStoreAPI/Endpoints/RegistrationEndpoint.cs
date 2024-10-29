using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Data;
using PetStoreLibrary;
using PetStoreAPI.Entities;
using Microsoft.EntityFrameworkCore; // Thêm dòng này
using System.Security.Cryptography;
using System.Text;

namespace PetStoreAPI.Endpoints
{
    public static class RegistrationEndpoint
    {
        public static RouteGroupBuilder MapRegistrationEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/registration");

            group.MapPost("/", async ([FromBody] UserRegistrationDTO userDto, PetStoreDbContext db) =>
            {
                // Kiểm tra xem người dùng đã tồn tại hay chưa
                if (await db.Users.AnyAsync(u => u.Username == userDto.Username))
                {
                    return Results.BadRequest("Username already exists.");
                }

                var user = new UserEntity
                {
                    Username = userDto.Username,
                    Password = HashPassword(userDto.Password), // Mã hóa mật khẩu
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    Address = userDto.Address
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();
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
