using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using PetStoreLibrary.DTOs;
using PetStoreAPI.Entities;
using Microsoft.Extensions.Logging;

namespace PetStoreAPI.Endpoints
{
    public static class CartEndpoint
    {
        public static RouteGroupBuilder MapCartEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/cart");

            // Add item to cart
            group.MapPost("/", async ([FromBody] CartItemDTO cartItem, PetStoreDbContext db, ILogger<Program> logger) =>
            {
                logger.LogInformation("Received request to add item to cart: {@CartItem}", cartItem);

                // Fetch the pet from the database
                var pet = await db.PetStores.FindAsync(cartItem.PetId);

                // Check if the pet exists
                if (pet == null)
                {
                    logger.LogWarning("Invalid PetId: {PetId}", cartItem.PetId);
                    return Results.BadRequest("Invalid PetId");
                }

                // Fetch the user from the database
                var user = await db.Users.FindAsync(cartItem.UserId);

                // Check if the user exists
                if (user == null)
                {
                    logger.LogWarning("Invalid UserId: {UserId}", cartItem.UserId);
                    return Results.BadRequest("Invalid UserId");
                }

                var item = new CartItemEntity
                {
                    PetId = cartItem.PetId,
                    Quantity = cartItem.Quantity,
                    DateAdded = DateOnly.FromDateTime(DateTime.Now),
                    UserId = cartItem.UserId,
                    Pet = pet, // Set the required Pet property
                    User = user // Set the required User property
                };

                db.CartItems.Add(item);
                await db.SaveChangesAsync();
                logger.LogInformation("Item added to cart: {@CartItemEntity}", item);
                return Results.Created($"/cart/{item.Id}", item);
            });

            // Get cart items for a specific user
            group.MapGet("/", async ([FromQuery] int userId, PetStoreDbContext db) =>
            {
                var cartItems = await db.CartItems
                    .Include(c => c.Pet)
                    .Where(c => c.UserId == userId)
                    .GroupBy(c => c.PetId)
                    .Select(g => new 
                    {
                        Pet = g.First().Pet,
                        Quantity = g.Sum(c => c.Quantity)
                    })
                    .ToListAsync();
                return Results.Ok(cartItems);
            });

            // Get cart count for a user
            group.MapGet("/count", async ([FromQuery] int userId, PetStoreDbContext db) =>
            {
                var count = await db.CartItems.CountAsync(c => c.UserId == userId);
                return Results.Ok(new { count });
            });

            return group;
        }
    }
}
