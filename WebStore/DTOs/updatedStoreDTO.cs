namespace WebStore.DTOs;
public record class UpdatedPetStoreDTO(
    int id, 
    string itemId,
    string productId,
    string petType, 
    string gender, 
    string petDescription,
    decimal price, 
    DateOnly birthDay);



