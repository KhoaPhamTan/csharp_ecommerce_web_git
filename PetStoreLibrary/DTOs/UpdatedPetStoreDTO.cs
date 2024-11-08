namespace PetStoreLibrary.DTOs;
public record UpdatedPetStoreDTO(
    string PetName, 
    string CategoryName, // Use CategoryName instead of CategoryDTO
    string Gender, 
    string PetDescription, 
    decimal Price, 
    DateOnly BirthDay,
    string ImageUrl);