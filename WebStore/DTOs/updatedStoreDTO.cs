using System.ComponentModel.DataAnnotations;

namespace WebStore.DTOs;
public record class UpdatedPetStoreDTO(
    int id, 
    [Required][StringLength(10)] string itemId,
    [Required][StringLength(10)] string productId,
    [Required][StringLength(10)] string petType,
    [Required][StringLength(6)] string gender,
    [Required][StringLength(255)] string petDescription,
    [Range(1,10000)] decimal price,
    DateOnly birthDay);



