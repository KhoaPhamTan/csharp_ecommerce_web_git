using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using PetStoreLibrary.DTOs;
using PetStoreAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace PetStoreAPI.Endpoints
{
    public static class CartEndpoint
    {
        public static RouteGroupBuilder MapCartEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/cart");

            // Thêm sản phẩm vào giỏ hàng
            group.MapPost("/", async ([FromBody] CartItemDTO cartItem, PetStoreDbContext db, HttpContext httpContext, ILogger<Program> logger) =>
            {
                var userId = httpContext.User.Identity?.Name;
                if (userId == null)
                {
                    logger.LogWarning("Unauthorized access attempt to add to cart.");
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
                        UserId = userId,
                        PetId = cartItem.PetId,
                        Quantity = cartItem.Quantity,
                        DateAdded = DateOnly.FromDateTime(DateTime.Now),
                        Pet = pet // Set the required Pet property
                    };

                    db.CartItems.Add(item);
                    await db.SaveChangesAsync();
                    logger.LogInformation("Added pet {PetId} to cart for user {UserId}", cartItem.PetId, userId);
                    return Results.Created($"/cart/{item.Id}", item);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error adding pet to cart for user {UserId}", userId);
                    return Results.Problem("Internal server error", statusCode: 500);
                }
            });

            // Lấy danh sách giỏ hàng
            group.MapGet("/", async (PetStoreDbContext db, HttpContext httpContext, ILogger<Program> logger) =>
            {
                var userId = httpContext.User.Identity?.Name;
                if (userId == null)
                {
                    logger.LogWarning("Unauthorized access attempt to get cart items.");
                    return Results.Unauthorized();
                }

                var cartItems = await db.CartItems
                    .Include(c => c.Pet)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                logger.LogInformation("Fetched cart items for user {UserId}", userId);
                return Results.Ok(cartItems);
            });

            // Lấy số lượng sản phẩm trong giỏ hàng
            group.MapGet("/count", async (PetStoreDbContext db, HttpContext httpContext, ILogger<Program> logger) =>
            {
                var userId = httpContext.User.Identity?.Name;
                if (userId == null)
                {
                    logger.LogWarning("Unauthorized access attempt to get cart count.");
                    return Results.Unauthorized();
                }

                try
                {
                    var cartCount = await db.CartItems
                        .Where(c => c.UserId == userId)
                        .SumAsync(c => c.Quantity);

                    logger.LogInformation("Fetched cart count for user {UserId}: {CartCount}", userId, cartCount);
                    return Results.Ok(cartCount);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error fetching cart count for user {UserId}", userId);
                    return Results.Problem("Internal server error", statusCode: 500);
                }
            });

            return group;
        }
    }
}
