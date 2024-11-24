namespace PetStoreLibrary.DTOs;

public record CartItemDTO(
    int PetId, 
    int Quantity,
    int UserId);
