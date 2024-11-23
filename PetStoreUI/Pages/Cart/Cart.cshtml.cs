using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStoreLibrary.DTOs;
using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Extensions;

namespace PetStoreUI.Pages.Cart
{
    public class CartModel : PageModel
    {
        private readonly PetStoreDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CartModel> _logger;

        public CartModel(PetStoreDbContext dbContext, IHttpClientFactory httpClientFactory, ILogger<CartModel> logger)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();

        public async Task OnGetAsync()
        {
            var userName = User.Identity?.Name;
            if (userName == null)
            {
                CartItems = new List<CartItemDTO>();
                return;
            }

            // Construct the API URL
            var apiUrl = "http://localhost:5134/Cart"; // Ensure this matches the API backend address
            var uiUrl = HttpContext.Request.GetDisplayUrl();
            _logger.LogInformation("Fetching cart items from API: {ApiUrl} for UI: {UiUrl}", apiUrl, uiUrl);

            // Fetch cart items from the API for the logged-in user
            var client = _httpClientFactory.CreateClient("API");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var cartItems = await response.Content.ReadFromJsonAsync<List<CartItemDTO>>();
                    CartItems = cartItems ?? new List<CartItemDTO>();
                }
                else
                {
                    _logger.LogError("Failed to fetch cart items from API: {ApiUrl} for UI: {UiUrl}. Status code: {StatusCode}", apiUrl, uiUrl, response.StatusCode);
                    CartItems = new List<CartItemDTO>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cart items from API: {ApiUrl} for UI: {UiUrl}", apiUrl, uiUrl);
                CartItems = new List<CartItemDTO>();
            }
        }
    }
}