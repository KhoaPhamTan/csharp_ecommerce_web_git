using PetStoreLibrary.DTOs;
using PetStoreAPI.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetStoreAPI.Endpoints
{
    public static class PetDetailsEndpoint
    {
        public static void MapPetEndpoints(this WebApplication app)
        {
            app.MapGet("/pets/{id}", async (int id, PetStoreDbContext db) =>
            {
                if (db.PetStores == null)
                {
                    return Results.NotFound();
                }

                var petStore = await db.PetStores
                    .Include(ps => ps.Category) // Include Category navigation property
                    .FirstOrDefaultAsync(ps => ps.Id == id);

                if (petStore == null)
                {
                    return Results.NotFound();
                }

                var petStoreDetailDTO = new PetStoreDetailDTO(
                    petStore.Id,
                    petStore.PetName,
                    new CategoryDTO(petStore.Category.Id, petStore.Category.Name), // Map Category to CategoryDTO
                    petStore.Gender,
                    petStore.PetDescription,
                    petStore.Price,
                    DateOnly.FromDateTime(petStore.BirthDay),
                    petStore.ImageUrl.StartsWith("/images/") ? petStore.ImageUrl : "/images/" + petStore.ImageUrl); // Ensure ImageUrl is correctly set

                return Results.Ok(petStoreDetailDTO);
            });

            // Other endpoint methods...
        }
    }
}
