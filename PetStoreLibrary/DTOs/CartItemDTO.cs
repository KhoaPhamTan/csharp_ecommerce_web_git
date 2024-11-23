namespace PetStoreLibrary.DTOs;

public record CartItemDTO(
    int PetId, 
    int Quantity,
    decimal Price, // Add Price property
    string PetName // Add PetName property
);
