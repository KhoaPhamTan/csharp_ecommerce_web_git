using PetStoreLibrary.DTOs;
using System.Collections.Generic; 
public class PetStoreViewModel
{
    public List<PetStoreDTO> PetStores { get; set; }
    public List<PetTypeDTO> PetTypes { get; set; }

    public PetStoreViewModel()
    {
        PetStores = new List<PetStoreDTO>(); // Khởi tạo danh sách rỗng
        PetTypes = new List<PetTypeDTO>();   // Khởi tạo danh sách rỗng
    }
}