
using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;


namespace PetStoreAPI.Endpoints;

public static class PetTypeEndpoint
{
    public static RouteGroupBuilder MapPetTypeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/petTypes");

        // GET: Lấy danh sách các loại thú cưng
        group.MapGet("/", async (PetStoreDbContext db) =>
        {
            var petTypes = await db.PetTypes.ToListAsync();
            return Results.Ok(petTypes);
        });

        return group;
    }
}
