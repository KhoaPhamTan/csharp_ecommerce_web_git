namespace PetStoreLibrary;
public record CreatePetDTO(
    string ItemId, 
    string ProductId, 
    int PetTypeId,  // Ensure this is included
    string Gender, 
    string PetDescription, 
    decimal Price, 
    DateOnly BirthDay);