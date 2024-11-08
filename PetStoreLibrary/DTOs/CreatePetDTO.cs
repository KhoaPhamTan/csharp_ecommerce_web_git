namespace PetStoreLibrary.DTOs;
public record CreatePetDTO(
    string PetName, 
    string CategoryName, // Use CategoryName instead of CategoryId
    string Gender, 
    string PetDescription, 
    decimal Price, 
    DateOnly BirthDay,
    string ImageUrl);