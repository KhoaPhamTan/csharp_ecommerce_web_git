using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using PetStoreLibrary.DTOs;
using PetStoreAPI.Entities;

namespace PetStoreAPI.Endpoints
{
    public static class CartEndpoint
    {
        public static RouteGroupBuilder MapCartEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/cart");

            // Thêm sản phẩm vào giỏ hàng
            group.MapPost("/", async ([FromBody] CartItemDTO cartItem, PetStoreDbContext db) =>
            {
                // Fetch the pet from the database
                var pet = await db.PetStores.FindAsync(cartItem.PetId);

                // Check if the pet exists
                if (pet == null)
                {
                    return Results.BadRequest("Invalid PetId");
                }

                var item = new CartItemEntity
                {
                    PetId = cartItem.PetId,
                    Quantity = cartItem.Quantity,
                    DateAdded = DateOnly.FromDateTime(DateTime.Now),
                    Pet = pet // Set the required Pet property
                };

                db.CartItems.Add(item);
                await db.SaveChangesAsync();
                return Results.Created($"/cart/{item.Id}", item);
            });


            // Lấy danh sách giỏ hàng
            group.MapGet("/", async (PetStoreDbContext db) =>
                Results.Ok(await db.CartItems.Include(c => c.Pet).ToListAsync()));

            return group;
        }
    }
}
