using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetStoreAPI.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public static class CheckoutEndpoints
{
    public static RouteGroupBuilder MapCheckoutEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/checkout");

        group.MapPost("/", async ([FromBody] CheckoutRequest checkoutRequest, PetStoreDbContext dbContext, ILogger<Program> logger, HttpContext context) =>
        {
            var emailClaim = context.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailClaim))
            {
                logger.LogWarning("Email claim not found.");
                return Results.BadRequest("Email claim not found.");
            }

            logger.LogInformation("Checkout endpoint called with phone: {Phone}", checkoutRequest.Phone);

            if (string.IsNullOrEmpty(checkoutRequest.Phone))
            {
                logger.LogWarning("Phone number is required.");
                return Results.BadRequest("Phone number is required.");
            }

            logger.LogInformation("Clearing cart items for user: {Email}", emailClaim);

            var cartItems = await dbContext.CartItems
                .Where(c => c.UserEmail == emailClaim)
                .ToListAsync();

            logger.LogInformation("Number of cart items to clear: {Count}", cartItems.Count);

            dbContext.CartItems.RemoveRange(cartItems);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Cart items cleared for user: {Email}", emailClaim);

            return Results.Ok("Checkout completed successfully.");
        });

        return group;
    }
}

public class CheckoutRequest
{
    public string Phone { get; set; }
}