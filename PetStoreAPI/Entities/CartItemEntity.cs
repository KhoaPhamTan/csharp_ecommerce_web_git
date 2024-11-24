namespace PetStoreAPI.Entities;

public class CartItemEntity
{
    public int Id { get; set; }
    public required int PetId { get; set; }
    public required int Quantity { get; set; }
    public required DateOnly DateAdded { get; set; }
    public required int UserId { get; set; }  // Ensure UserId is of type int

    public required PetStoreEntity Pet { get; set; }  // Mark as required
    public required UserEntity User { get; set; }  // Add User navigation property
}