namespace PetStoreAPI.Entities;

public class CartItemEntity
{
    public int Id { get; set; }
    public required string UserId { get; set; } // Add UserId property
    public required int PetId { get; set; }
    public required int Quantity { get; set; }
    public required DateOnly DateAdded { get; set; }

    public required PetStoreEntity Pet { get; set; }  // Mark as required
}