namespace PetStoreLibrary.DTOs;

public class PetStoreDTO
{
    public int Id { get; set; }
    public string PetName { get; set; }
    public string CategoryName { get; set; } // Ensure this matches the category names in the database
    public string Gender { get; set; }
    public string PetDescription { get; set; }
    public decimal Price { get; set; }
    public DateOnly BirthDay { get; set; }
    public string ImageUrl { get; set; }

    public PetStoreDTO(int id, string petName, string categoryName, string gender, string petDescription, decimal price, DateOnly birthDay, string imageUrl)
    {
        Id = id;
        PetName = petName;
        CategoryName = categoryName;
        Gender = gender;
        PetDescription = petDescription;
        Price = price;
        BirthDay = birthDay;
        ImageUrl = imageUrl;
    }
}