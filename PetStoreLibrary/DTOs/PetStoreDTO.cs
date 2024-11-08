namespace PetStoreLibrary.DTOs;

public record PetStoreDTO(
    int Id, 
    string PetName, 
    CategoryDTO Category, // Link to CategoryDTO
    string Gender, 
    string PetDescription, 
    decimal Price, 
    DateOnly BirthDay,
    string ImageUrl);
