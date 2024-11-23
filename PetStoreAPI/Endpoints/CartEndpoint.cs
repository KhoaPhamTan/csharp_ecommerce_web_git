using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using PetStoreLibrary.DTOs;
using PetStoreAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace PetStoreAPI.Endpoints
{
    public static class CartEndpoint
    {
        public static RouteGroupBuilder MapCartEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/Cart").RequireCors("AllowSpecificOrigin"); // Ensure the base path is correctly set to /Cart and CORS policy is applied

            // Thêm sản phẩm vào giỏ hàng
            group.MapPost("/", [Authorize] async ([FromBody] CartItemDTO cartItem, PetStoreDbContext db, HttpContext httpContext, ILogger<Program> logger) =>
            {
                var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                if (userEmail == null)
                {
                    logger.LogWarning("Unauthorized access attempt to add to cart.");
                    return Results.Unauthorized();
                }

                if (db.Users == null)
                {
                    logger.LogWarning("Users DbSet is null.");
                    return Results.Problem("Internal server error", statusCode: 500);
                }

                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null)
                {
                    logger.LogWarning("User not found: {UserEmail}", userEmail);
                    return Results.Unauthorized();
                }

                try
                {
                    // Fetch the pet from the database
                    var pet = await db.PetStores.FindAsync(cartItem.PetId);

                    // Check if the pet exists
                    if (pet == null)
                    {
                        logger.LogWarning("Invalid PetId: {PetId}", cartItem.PetId);
                        return Results.BadRequest("Invalid PetId");
                    }

                    var item = new CartItemEntity
                    {
                        UserId = user.Id,
                        PetId = cartItem.PetId,
                        Quantity = cartItem.Quantity,
                        DateAdded = DateOnly.FromDateTime(DateTime.Now),
                        Pet = pet // Set the required Pet property
                    };

                    db.CartItems.Add(item);
                    await db.SaveChangesAsync();
                    logger.LogInformation("Added pet {PetId} to cart for user {UserId}", cartItem.PetId, user.Id);
                    return Results.Created($"/Cart/{item.Id}", item); // Ensure the URL is correctly set
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error adding pet to cart for user {UserId}", user.Id);
                    return Results.Problem("Internal server error", statusCode: 500);
                }
            });

            // Lấy danh sách giỏ hàng
            group.MapGet("/", [Authorize] async (PetStoreDbContext db, HttpContext httpContext, ILogger<Program> logger) =>
            {
                var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                if (userEmail == null)
                {
                    logger.LogWarning("Unauthorized access attempt to get cart items.");
                    return Results.Unauthorized();
                }

                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null)
                {
                    logger.LogWarning("User not found: {UserEmail}", userEmail);
                    return Results.Unauthorized();
                }

                var cartItems = await db.CartItems
                    .Include(c => c.Pet)
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();

                // Map từ CartItemEntity sang CartItemDTO
                var cartItemDTOs = cartItems.Select(item => new CartItemDTO(
                    PetId: item.PetId,
                    Quantity: item.Quantity,
                    Price: item.Pet.Price, // Truyền giá trị Price từ Pet entity
                    PetName: item.Pet.PetName // Truyền tên pet từ Pet entity
                )).ToList();

                logger.LogInformation("Fetched cart items for user {UserId}", user.Id);
                return Results.Ok(cartItemDTOs);
            });

            // Lấy số lượng sản phẩm trong giỏ hàng
            group.MapGet("/count", [Authorize] async (PetStoreDbContext db, HttpContext httpContext, ILogger<Program> logger) =>
            {
                var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                logger.LogInformation("Fetching cart count for user {UserEmail}", userEmail);

                if (userEmail == null)
                {
                    logger.LogWarning("Unauthorized access attempt to get cart count.");
                    return Results.Unauthorized();
                }

                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null)
                {
                    logger.LogWarning("User not found: {UserEmail}", userEmail);
                    return Results.Unauthorized();
                }

                try
                {
                    var cartCount = await db.CartItems
                        .Where(c => c.UserId == user.Id)
                        .SumAsync(c => c.Quantity);

                    logger.LogInformation("Fetched cart count for user {UserId}: {CartCount}", user.Id, cartCount);
                    return Results.Ok(new { count = cartCount });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error fetching cart count for user {UserId}", user.Id);
                    return Results.Problem("Internal server error", statusCode: 500);
                }
            });

            return group;
        }
    }
}
