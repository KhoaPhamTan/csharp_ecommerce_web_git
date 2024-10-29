namespace PetStoreAPI.Entities;

public class PetStoreEntity
{
    public int Id { get; set; }
    public required string ItemId { get; set; }
    public required string ProductId { get; set; }
    public int PetTypeId { get; set; }
    public required string Gender { get; set; }
    public required string PetDescription { get; set; }
    public required decimal Price { get; set; }
    public required DateTime BirthDay { get; set; }
    public required PetTypeEntity PetType { get; set; }  // Quan hệ với PetTypeEntity
}
