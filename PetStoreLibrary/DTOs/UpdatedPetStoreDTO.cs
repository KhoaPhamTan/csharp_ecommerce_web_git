namespace PetStoreLibrary;
public record UpdatedPetStoreDTO(
    int Id, 
    string ItemId, 
    string ProductId, 
    int PetTypeId,  // Ensure this is included
    string Gender, 
    string PetDescription, 
    decimal Price, 
    DateOnly BirthDay);