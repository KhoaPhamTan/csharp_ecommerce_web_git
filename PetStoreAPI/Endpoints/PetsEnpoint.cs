using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;

namespace PetStoreAPI.Endpoints
{
    public static class PetEndpoints
    {
        public static RouteGroupBuilder MapPetEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/pets");

            // GET: Lấy danh sách thú cưng theo loại
            group.MapGet("/", async (int petTypeId, PetStoreDbContext db) =>
            {
                var pets = await db.PetStores
                    .Include(p => p.PetType)
                    .Where(p => p.PetTypeId == petTypeId)
                    .ToListAsync();

                if (!pets.Any())
                {
                    return Results.NotFound(new { Message = "No pets found for this type." });
                }

                var petDtos = pets.Select(p => new PetStoreDTO(
                    p.Id,
                    p.ItemId,
                    p.ProductId,
                    p.PetType.Name,
                    p.Gender,
                    p.PetDescription,
                    p.Price,
                    DateOnly.FromDateTime(p.BirthDay)
                )).ToList();

                return Results.Ok(petDtos);
            });

            return group;
        }
    }
}
