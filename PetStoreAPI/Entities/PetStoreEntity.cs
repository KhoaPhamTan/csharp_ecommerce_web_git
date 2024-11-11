namespace PetStoreAPI.Entities
{
    public class PetStoreEntity
    {
        public int Id { get; set; }
        public required string PetName { get; set; }
        public required string Gender { get; set; }
        public required string PetDescription { get; set; }
        public required decimal Price { get; set; }
        public required DateTime BirthDay { get; set; }
        public required string ImageUrl { get; set; }
        public required CategoryEntity Category { get; set; } // Use CategoryEntity
    }
}