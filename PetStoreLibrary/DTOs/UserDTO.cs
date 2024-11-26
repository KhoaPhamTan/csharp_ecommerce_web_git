namespace PetStoreLibrary.DTOs
{
    public record UserDTO(
        int Id,
        string Username,
        string FullName,
        string Email,
        string Address,
        string Role,
        string Password
    );
}