using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Entities;
using PetStoreLibrary.DTOs;

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
                var pets = await db.PetStores.Include(pet => pet.Category).ToListAsync();
                if (!pets.Any())
                {
                    return Results.NotFound(new { Message = "No pet stores found." });
                }

                var petDtos = pets.Select(pet => new PetStoreDTO(
                    pet.Id,
                    pet.PetName,
                    pet.Category.Name, // Use Category name directly
                    pet.Gender,
                    pet.PetDescription,
                    pet.Price,
                    DateOnly.FromDateTime(pet.BirthDay), // Convert DateTime to DateOnly
                    pet.ImageUrl // Add ImageUrl property
                )).ToList();
                
                return Results.Ok(petDtos);
            });

            // GET: Lấy thông tin pet store theo ID
            group.MapGet("/{id}", async (int id, PetStoreDbContext db) =>
            {
                var pet = await db.PetStores.Include(pet => pet.Category).FirstOrDefaultAsync(pet => pet.Id == id);
                if (pet is null)
                {
                    return Results.NotFound(new { Message = "Pet store not found." });
                }

                var petDto = new PetStoreDTO(
                    pet.Id,
                    pet.PetName,
                    pet.Category.Name, // Use Category name directly
                    pet.Gender,
                    pet.PetDescription,
                    pet.Price,
                    DateOnly.FromDateTime(pet.BirthDay),
                    pet.ImageUrl // Add ImageUrl property
                );

                return Results.Ok(petDto);
            });

            // POST: Tạo pet store mới
            group.MapPost("/", async ([FromBody] CreatePetDTO newPet, PetStoreDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(newPet.PetName) || 
                    string.IsNullOrWhiteSpace(newPet.Gender) || 
                    string.IsNullOrWhiteSpace(newPet.PetDescription) || 
                    newPet.Price <= 0)
                {
                    return Results.BadRequest(new { Message = "All fields are required and price must be positive." });
                }

                var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == newPet.CategoryName);
                if (category == null)
                {
                    return Results.BadRequest(new { Message = "Invalid Category." });
                }

                var pet = new PetStoreEntity
                {
                    PetName = newPet.PetName,
                    Category = category, // Use Category
                    Gender = newPet.Gender,
                    PetDescription = newPet.PetDescription,
                    Price = newPet.Price,
                    BirthDay = newPet.BirthDay.ToDateTime(TimeOnly.MinValue),
                    ImageUrl = newPet.ImageUrl // Add ImageUrl property
                };

                db.PetStores.Add(pet);
                await db.SaveChangesAsync();
                return Results.Created($"/petStores/{pet.Id}", pet);
            });

            // PUT: Cập nhật pet store theo ID
            group.MapPut("/{id}", async (int id, [FromBody] UpdatedPetStoreDTO updatedPet, PetStoreDbContext db) =>
            {
                var pet = await db.PetStores.Include(pet => pet.Category).FirstOrDefaultAsync(pet => pet.Id == id);
                if (pet == null)
                {
                    return Results.NotFound(new { Message = "Pet store not found." });
                }

                if (string.IsNullOrWhiteSpace(updatedPet.PetName) || 
                    string.IsNullOrWhiteSpace(updatedPet.Gender) || 
                    string.IsNullOrWhiteSpace(updatedPet.PetDescription) || 
                    updatedPet.Price <= 0)
                {
                    return Results.BadRequest(new { Message = "All fields are required and price must be positive." });
                }

                var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == updatedPet.CategoryName);
                if (category == null)
                {
                    return Results.BadRequest(new { Message = "Invalid Category." });
                }

                pet.PetName = updatedPet.PetName;
                pet.Category = category; // Use Category
                pet.Gender = updatedPet.Gender;
                pet.PetDescription = updatedPet.PetDescription;
                pet.Price = updatedPet.Price;
                pet.BirthDay = updatedPet.BirthDay.ToDateTime(TimeOnly.MinValue);
                pet.ImageUrl = updatedPet.ImageUrl; // Add ImageUrl property

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
