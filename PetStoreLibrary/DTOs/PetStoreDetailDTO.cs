namespace PetStoreLibrary.DTOs;
public record PetStoreDetailDTO(
    int Id,
    string ItemId,
    string ProductId,
    string PetType,
    string Gender,
    string PetDescription,
    decimal Price,
    DateOnly BirthDay);
