using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStoreLibrary.DTOs;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PetStoreUI.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(ILogger<RegisterModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public new UserRegistrationDTO? User { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _logger.LogInformation("Sending registration request for user: {Username}", User?.Username);

            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                using var client = new HttpClient(handler);
                var content = new StringContent(JsonSerializer.Serialize(User), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:5134/registration", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Registration successful for user: {Username}", User?.Username);
                    return RedirectToPage("/Account/Login");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Registration failed for user: {Username}. Response: {Response}", User?.Username, responseContent);
                ModelState.AddModelError(string.Empty, $"Registration failed: {responseContent}");
                ErrorMessage = $"Registration failed: {responseContent}";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error during registration for user: {Username}", User?.Username);
                ErrorMessage = "An error occurred while sending the registration request. Please try again later.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for user: {Username}", User?.Username);
                ErrorMessage = "An unexpected error occurred. Please try again later.";
            }

            return Page();
        }
    }
}