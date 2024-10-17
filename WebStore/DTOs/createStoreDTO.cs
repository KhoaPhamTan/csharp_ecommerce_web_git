namespace WebStore.DTOs;
public record class CreatePetDTO(
    string itemId,
    string productId,
    string petType, 
    string gender, 
    string petDescription,
    decimal price, 
    DateOnly birthDay);

