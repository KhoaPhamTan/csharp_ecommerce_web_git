using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Entities;
using PetStoreLibrary.DTOs;
using System.IO;
using Microsoft.Extensions.Logging;

namespace PetStoreAPI.EndPoints
{
    public static class StoreEndpoints
    {
        public static RouteGroupBuilder MapStoresEndPoints(this WebApplication app)
        {
            var group = app.MapGroup("/petStores");

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
                    pet.Category.Name,
                    pet.Gender,
                    pet.PetDescription,
                    pet.Price,
                    DateOnly.FromDateTime(pet.BirthDay),
                    pet.ImageUrl.StartsWith("/images/") ? pet.ImageUrl : "/images/" + pet.ImageUrl // Ensure ImageUrl is correctly set
                )).ToList();

                return Results.Ok(petDtos);
            });

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
                    pet.Category.Name,
                    pet.Gender,
                    pet.PetDescription,
                    pet.Price,
                    DateOnly.FromDateTime(pet.BirthDay),
                    pet.ImageUrl.StartsWith("/images/") ? pet.ImageUrl : "/images/" + pet.ImageUrl // Ensure ImageUrl is correctly set
                );

                return Results.Ok(petDto);
            });

            group.MapPost("/", async ([FromBody] CreatePetDTO newPet, PetStoreDbContext db, ILogger<ILogger> logger) =>
            {
                if (string.IsNullOrWhiteSpace(newPet.PetName))
                {
                    logger.LogError("Pet name is required.");
                    return Results.BadRequest(new { Message = "Pet name is required." });
                }
                if (string.IsNullOrWhiteSpace(newPet.Gender))
                {
                    logger.LogError("Gender is required.");
                    return Results.BadRequest(new { Message = "Gender is required." });
                }
                if (string.IsNullOrWhiteSpace(newPet.PetDescription))
                {
                    logger.LogError("Description is required.");
                    return Results.BadRequest(new { Message = "Description is required." });
                }
                if (newPet.Price <= 0)
                {
                    logger.LogError("Price must be positive.");
                    return Results.BadRequest(new { Message = "Price must be positive." });
                }
                if (string.IsNullOrWhiteSpace(newPet.CategoryName))
                {
                    logger.LogError("Category name is required.");
                    return Results.BadRequest(new { Message = "Category name is required." });
                }
                if (newPet.BirthDay == default)
                {
                    logger.LogError("Birth date is required.");
                    return Results.BadRequest(new { Message = "Birth date is required." });
                }

                var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == newPet.CategoryName);
                if (category == null)
                {
                    logger.LogError("Invalid Category.");
                    return Results.BadRequest(new { Message = "Invalid Category." });
                }

                var pet = new PetStoreEntity
                {
                    PetName = newPet.PetName,
                    Category = category,
                    Gender = newPet.Gender,
                    PetDescription = newPet.PetDescription,
                    Price = newPet.Price,
                    BirthDay = newPet.BirthDay.ToDateTime(TimeOnly.MinValue),
                    ImageUrl = newPet.ImageUrl // Save the image URL as provided
                };

                db.PetStores.Add(pet);
                await db.SaveChangesAsync();
                return Results.Created($"/petStores/{pet.Id}", pet);
            });

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
                pet.Category = category;
                pet.Gender = updatedPet.Gender;
                pet.PetDescription = updatedPet.PetDescription;
                pet.Price = updatedPet.Price;
                pet.BirthDay = updatedPet.BirthDay.ToDateTime(TimeOnly.MinValue);
                pet.ImageUrl = updatedPet.ImageUrl; // Save the image URL as provided

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

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

            // New endpoint to handle image URL update
            group.MapPost("/upload", async (HttpRequest request, PetStoreDbContext db, ILogger<ILogger> logger) =>
            {
                var form = await request.ReadFormAsync();
                var file = form.Files["file"];
                var petIdString = form["petId"];
                int petId = 0;

                if (!int.TryParse(petIdString, out petId))
                {
                    logger.LogError("Invalid or missing pet ID.");
                    return Results.BadRequest(new { Message = "Invalid or missing pet ID." });
                }

                if (file == null || file.Length == 0)
                {
                    logger.LogError("No file uploaded.");
                    return Results.BadRequest(new { Message = "No file uploaded." });
                }

                var originalFileName = file.FileName;
                var originalDirectory = Path.GetDirectoryName(originalFileName)?.Replace("\\", "/");
                var fileName = Path.Combine("images", originalDirectory, Path.GetFileName(originalFileName)).Replace("\\", "/");
                var filePath = Path.Combine("wwwroot", fileName);

                // Ensure the directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                if (petId != 0)
                {
                    var pet = await db.PetStores.FindAsync(petId);
                    if (pet == null)
                    {
                        logger.LogError($"Pet with ID {petId} not found.");
                        return Results.NotFound(new { Message = "Pet not found." });
                    }

                    pet.ImageUrl = fileName; // Save the relative path in the database
                    await db.SaveChangesAsync();
                    logger.LogInformation($"Pet with ID {petId} updated with new image URL: {fileName}");
                }

                return Results.Ok(new { ImageUrl = "/" + fileName }); // Ensure consistency in the returned URL
            });

            return group;
        }
    }
}
