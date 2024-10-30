namespace PetStoreLibrary;

public record PetStoreDTO(
    int Id, 
    string ItemId, 
    string ProductId, 
    string PetType,                     
    string Gender, 
    string PetDescription, 
    decimal Price, 
    DateOnly BirthDay);
