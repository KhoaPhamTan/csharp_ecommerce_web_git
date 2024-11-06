namespace PetStoreLibrary.DTOs;

public record PetStoreDTO(
    int Id, 
    string ItemId, 
    string ProductId, 
    string PetType, // Duy trì là chuỗi
    string Gender, 
    string PetDescription, 
    decimal Price, 
    DateOnly BirthDay);
