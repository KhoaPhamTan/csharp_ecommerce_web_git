using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using PetStoreLibrary.DTOs;
using PetStoreAPI.Entities;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

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
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == cartItem.UserEmail);

                // Check if the user exists
                if (user == null)
                {
                    logger.LogWarning("Invalid UserEmail: {UserEmail}", cartItem.UserEmail);
                    return Results.BadRequest("Invalid UserEmail");
                }

                // Check if the pet already exists in the cart
                var existingCartItem = await db.CartItems.FirstOrDefaultAsync(c => c.PetId == cartItem.PetId && c.UserEmail == cartItem.UserEmail);

                if (existingCartItem != null)
                {
                    // Update the quantity if the pet already exists in the cart
                    existingCartItem.Quantity += cartItem.Quantity;
                    await db.SaveChangesAsync();
                    logger.LogInformation("Updated quantity for existing cart item: {@CartItemEntity}", existingCartItem);
                    return Results.Ok(existingCartItem);
                }
                else
                {
                    // Add a new cart item if the pet does not exist in the cart
                    var item = new CartItemEntity
                    {
                        PetId = cartItem.PetId,
                        Quantity = cartItem.Quantity,
                        DateAdded = DateOnly.FromDateTime(DateTime.Now),
                        UserEmail = cartItem.UserEmail, // Use UserEmail instead of UserId
                        UserId = user.Id, // Set the required UserId property
                        Pet = pet, // Set the required Pet property
                        User = user // Set the required User property
                    };

                    db.CartItems.Add(item);
                    await db.SaveChangesAsync();
                    logger.LogInformation("Item added to cart: {@CartItemEntity}", item);
                    return Results.Created($"/cart/{item.Id}", item);
                }
            });

            // Update cart item quantity
            group.MapPost("/update", async ([FromBody] CartItemDTO cartItem, PetStoreDbContext db, ILogger<Program> logger) =>
            {
                logger.LogInformation("Received request to update cart item quantity: {@CartItem}", cartItem);

                // Fetch the cart item from the database
                var item = await db.CartItems.FirstOrDefaultAsync(c => c.Id == cartItem.CartItemId && c.UserEmail == cartItem.UserEmail);

                // Check if the cart item exists
                if (item == null)
                {
                    logger.LogWarning("Invalid CartItemId or UserEmail: {CartItemId}, {UserEmail}", cartItem.CartItemId, cartItem.UserEmail);
                    return Results.BadRequest("Invalid CartItemId or UserEmail");
                }

                // Update the quantity
                item.Quantity = cartItem.Quantity;
                await db.SaveChangesAsync();

                logger.LogInformation("Cart item quantity updated: {@CartItemEntity}", item);
                return Results.Ok(item);
            });

            // Remove item from cart
            group.MapPost("/remove", async ([FromBody] CartItemDTO cartItem, PetStoreDbContext db, ILogger<Program> logger) =>
            {
                logger.LogInformation("Received request to remove cart item: {@CartItem}", cartItem);

                // Fetch the cart item from the database
                var item = await db.CartItems.FirstOrDefaultAsync(c => c.Id == cartItem.CartItemId && c.UserEmail == cartItem.UserEmail);

                // Check if the cart item exists
                if (item == null)
                {
                    logger.LogWarning("Invalid CartItemId or UserEmail: {CartItemId}, {UserEmail}", cartItem.CartItemId, cartItem.UserEmail);
                    return Results.BadRequest("Invalid CartItemId or UserEmail");
                }

                // Remove the cart item
                db.CartItems.Remove(item);
                await db.SaveChangesAsync();

                logger.LogInformation("Cart item removed: {@CartItemEntity}", item);
                return Results.Ok();
            });

            // Get cart items for a specific user
            group.MapGet("/", async (HttpContext httpContext, PetStoreDbContext db, ILogger<Program> logger) =>
            {
                // Check if the user is authenticated
                if (!httpContext.User.Identity.IsAuthenticated)
                {
                    logger.LogWarning("Unauthorized access attempt.");
                    return Results.Unauthorized();
                }

                // Get email from Claims
                var emailClaim = httpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
                var userNameClaim = httpContext.User?.FindFirst(ClaimTypes.Name)?.Value;

                logger.LogInformation($"Email from Claims: {emailClaim}");
                logger.LogInformation($"User Name from Claims: {userNameClaim}");

                // Log all claims for debugging
                foreach (var claim in httpContext.User.Claims)
                {
                    logger.LogInformation($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }

                if (string.IsNullOrEmpty(emailClaim))
                {
                    logger.LogError("Email not found in Claims.");
                    return Results.BadRequest("Email is required.");
                }

                // Fetch the user using the email
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);
                if (user == null)
                {
                    logger.LogError("User not found for the provided email.");
                    return Results.BadRequest("User not found.");
                }

                var userEmail = emailClaim;

                // Get the cart items for the user
                var cartItems = await db.CartItems
                    .Include(c => c.Pet)
                    .Where(c => c.UserEmail == userEmail)
                    .GroupBy(c => c.PetId)
                    .Select(g => new
                    {
                        CartItemId = g.First().Id,
                        Pet = g.First().Pet,
                        Quantity = g.Sum(c => c.Quantity)
                    })
                    .ToListAsync();

                return Results.Ok(cartItems);
            });

            // Get cart count for a user
            group.MapGet("/count", async (HttpContext httpContext, PetStoreDbContext db, ILogger<Program> logger) =>
            {
                // Get email from Claims
                var emailClaim = httpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
                var userNameClaim = httpContext.User?.FindFirst(ClaimTypes.Name)?.Value;

                logger.LogInformation($"Email from Claims: {emailClaim}");
                logger.LogInformation($"User Name from Claims: {userNameClaim}");

                // Log all claims for debugging
                foreach (var claim in httpContext.User.Claims)
                {
                    logger.LogInformation($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }

                if (string.IsNullOrEmpty(emailClaim))
                {
                    logger.LogError("Email not found in Claims.");
                    return Results.BadRequest("Email is required.");
                }

                // Fetch the user using the email
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);
                if (user == null)
                {
                    logger.LogError("User not found for the provided email.");
                    return Results.BadRequest("User not found.");
                }

                var userEmail = emailClaim;

                // Calculate the total quantity of items in the cart
                var totalQuantity = await db.CartItems
                    .Where(c => c.UserEmail == userEmail)
                    .SumAsync(c => c.Quantity);

                return Results.Ok(new { count = totalQuantity });
            });

            return group;
        }
    }
}
