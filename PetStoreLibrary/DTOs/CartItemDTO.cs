namespace PetStoreLibrary.DTOs;

public class CartItemDTO
{
    public int CartItemId { get; set; } // Add CartItemId property
    public int PetId { get; set; }
    public int Quantity { get; set; }
    public int UserId { get; set; } // Keep UserId
    public string UserEmail { get; set; } // Rename UserId_int to UserEmail
}
