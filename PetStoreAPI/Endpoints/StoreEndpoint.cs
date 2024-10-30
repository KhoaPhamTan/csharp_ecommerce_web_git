using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Entities;
using PetStoreLibrary;

namespace PetStoreAPI.EndPoints
{
    public static class StoreEndpoints
    {
        public static RouteGroupBuilder MapStoresEndPoints(this WebApplication app)
        {
            var group = app.MapGroup("/petStores");

            // GET: Lấy danh sách tất cả pet stores
            group.MapGet("/", async (PetStoreDbContext db) =>
            {
                var pets = await db.PetStores.Include(p => p.PetType).ToListAsync();
                if (!pets.Any())
                {
                    return Results.NotFound(new { Message = "No pet stores found." });
                }

                var petDtos = pets.Select(p => new PetStoreDTO(
                    p.Id,
                    p.ItemId,
                    p.ProductId,
                    p.PetType.Name,
                    p.Gender,
                    p.PetDescription,
                    p.Price,
                    DateOnly.FromDateTime(p.BirthDay) // Convert DateTime to DateOnly
                )).ToList();
                
                return Results.Ok(petDtos);
            });

            // GET: Lấy thông tin pet store theo ID
            group.MapGet("/{id}", async (int id, PetStoreDbContext db) =>
            {
                var pet = await db.PetStores.Include(p => p.PetType).FirstOrDefaultAsync(p => p.Id == id);
                if (pet is null)
                {
                    return Results.NotFound(new { Message = "Pet store not found." });
                }

                var petDto = new PetStoreDTO(
                    pet.Id,
                    pet.ItemId,
                    pet.ProductId,
                    pet.PetType.Name,
                    pet.Gender,
                    pet.PetDescription,
                    pet.Price,
                    DateOnly.FromDateTime(pet.BirthDay)
                );

                return Results.Ok(petDto);
            });

            // POST: Tạo pet store mới
            group.MapPost("/", async ([FromBody] CreatePetDTO newPet, PetStoreDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(newPet.ItemId) || 
                    string.IsNullOrWhiteSpace(newPet.ProductId) || 
                    newPet.PetTypeId <= 0 || 
                    string.IsNullOrWhiteSpace(newPet.Gender) || 
                    string.IsNullOrWhiteSpace(newPet.PetDescription) || 
                    newPet.Price <= 0)
                {
                    return Results.BadRequest(new { Message = "All fields are required and price must be positive." });
                }

                var petType = await db.PetTypes.FindAsync(newPet.PetTypeId);
                if (petType == null)
                {
                    return Results.BadRequest(new { Message = "Invalid PetTypeId." });
                }

                var pet = new PetStoreEntity
                {
                    ItemId = newPet.ItemId,
                    ProductId = newPet.ProductId,
                    PetTypeId = newPet.PetTypeId,
                    PetType = petType,
                    Gender = newPet.Gender,
                    PetDescription = newPet.PetDescription,
                    Price = newPet.Price,
                    BirthDay = newPet.BirthDay.ToDateTime(TimeOnly.MinValue)
                };

                db.PetStores.Add(pet);
                await db.SaveChangesAsync();
                return Results.Created($"/petStores/{pet.Id}", pet);
            });

            // PUT: Cập nhật pet store theo ID
            group.MapPut("/{id}", async (int id, [FromBody] UpdatedPetStoreDTO updatedPet, PetStoreDbContext db) =>
            {
                var pet = await db.PetStores.FindAsync(id);
                if (pet == null)
                {
                    return Results.NotFound(new { Message = "Pet store not found." });
                }

                if (string.IsNullOrWhiteSpace(updatedPet.ItemId) || 
                    string.IsNullOrWhiteSpace(updatedPet.ProductId) || 
                    updatedPet.PetTypeId <= 0 || 
                    string.IsNullOrWhiteSpace(updatedPet.Gender) || 
                    string.IsNullOrWhiteSpace(updatedPet.PetDescription) || 
                    updatedPet.Price <= 0)
                {
                    return Results.BadRequest(new { Message = "All fields are required and price must be positive." });
                }

                pet.ItemId = updatedPet.ItemId;
                pet.ProductId = updatedPet.ProductId;
                pet.PetTypeId = updatedPet.PetTypeId;
                pet.Gender = updatedPet.Gender;
                pet.PetDescription = updatedPet.PetDescription;
                pet.Price = updatedPet.Price;
                pet.BirthDay = updatedPet.BirthDay.ToDateTime(TimeOnly.MinValue);

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // DELETE: Xóa pet store theo ID
            group.MapDelete("/{id}", async (int id, PetStoreDbContext db) =>
            {
                var pet = await db.PetStores.FindAsync(id);
                if (pet == null)
                {
                    return Results.NotFound(new { Message = "Pet store not found." });
                }

                db.PetStores.Remove(pet);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            return group;
        }
    }
}
