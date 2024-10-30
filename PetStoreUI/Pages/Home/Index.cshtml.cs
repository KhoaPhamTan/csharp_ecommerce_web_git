using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStoreLibrary;
using System.Net.Http.Json;

public class PetsIndexModel : PageModel
{
    public List<PetTypeDTO> PetTypes { get; set; } = new();

    private readonly HttpClient _httpClient;

    public PetsIndexModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task OnGetAsync()
    {
        PetTypes = await _httpClient.GetFromJsonAsync<List<PetTypeDTO>>("http://localhost:5134/petTypes") ?? new();
    }
}