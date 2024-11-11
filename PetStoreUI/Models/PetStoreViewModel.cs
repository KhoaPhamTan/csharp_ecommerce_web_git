namespace PetStoreUI.Models
{
    public class PetStoreViewModel
    {
        public int Id { get; set; }
        public string? PetName { get; set; }
        public string? Gender { get; set; }
        public string? PetDescription { get; set; }
        public decimal Price { get; set; }
        public DateTime BirthDay { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }
    }
}
