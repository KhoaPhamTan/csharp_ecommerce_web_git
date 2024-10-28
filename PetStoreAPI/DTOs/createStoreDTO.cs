using System.ComponentModel.DataAnnotations;

namespace WebStore.DTOs;
public record class CreatePetDTO(
    [Required][StringLength(10)] string ItemId,
    [Required][StringLength(10)] string ProductId,
    [Required][StringLength(10)] string PetType,
    [Required][StringLength(6)] string Gender,
    [Required][StringLength(255)] string PetDescription,
    [Range(1,10000)] decimal Price,
    DateOnly BirthDay);

