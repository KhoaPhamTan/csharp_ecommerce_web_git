namespace PetStoreAPI.Entities;

public class CartItemEntity
{
    public int Id { get; set; }
    public int UserId { get; set; } // Change type to int
    public int PetId { get; set; }
    public int Quantity { get; set; }
    public DateOnly DateAdded { get; set; }
    public PetStoreEntity Pet { get; set; }
}