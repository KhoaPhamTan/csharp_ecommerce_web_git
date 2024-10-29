using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStoreAPI.Services; // Kiểm tra xem tên dịch vụ có đúng không
using PetStoreLibrary; // Kiểm tra xem namespace cho DTOs

public class IndexModel : PageModel
{
    private readonly PetStoreService _petService;

    public IndexModel(PetStoreService petService)
    {
        _petService = petService;
    }

    public async Task OnGetAsync()
    {
        var pets = await _petService.GetAllPetsAsync();
    }
}
