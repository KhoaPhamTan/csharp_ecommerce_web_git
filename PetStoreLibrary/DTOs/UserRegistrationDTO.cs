namespace PetStoreLibrary.DTOs;
public record UserRegistrationDTO(
    string Username, 
    string Password, 
    string FullName, 
    string Email, 
    string Address);
