using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Entities;

namespace PetStoreAPI.Endpoints
{
    public static class CategoryEndpoints
    {
        public static RouteGroupBuilder MapCategoryEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/categories");

            // GET: Lấy danh sách tất cả categories
            group.MapGet("/", async (PetStoreDbContext db) =>
            {
                var categories = await db.Categories.ToListAsync();
                if (!categories.Any())
                {
                    return Results.NotFound(new { Message = "No categories found." });
                }

                return Results.Ok(categories);
            });

            return group;
        }
    }
}