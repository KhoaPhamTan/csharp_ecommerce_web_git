namespace PetStoreLibrary.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; } // Determines if the authentication session should persist across browser sessions
    }
}