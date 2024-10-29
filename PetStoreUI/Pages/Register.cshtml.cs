using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStoreLibrary;


public class RegisterModel : PageModel
{
    private readonly HttpClient _httpClient;

    public RegisterModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task OnPostAsync(string username, string password, string fullName, string email, string address)
    {
        var userDto = new UserRegistrationDTO(username, password, fullName, email, address);
        await _httpClient.PostAsJsonAsync("/registration", userDto);
    }
}
