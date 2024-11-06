using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using PetStoreLibrary.DTOs; // Tham chiếu đến Library

namespace PetStore.Controllers
{
    public class PetStoreController : Controller
    {
        private readonly HttpClient _httpClient;

        public PetStoreController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Hiển thị danh sách thú cưng
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("http://localhost:5134/petStores");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var pets = JsonSerializer.Deserialize<List<PetStoreDTO>>(content);
                return View(pets);
            }
            return View(new List<PetStoreDTO>());
        }

        // Hiển thị thú cưng theo loại
        public async Task<IActionResult> Category(int categoryId)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5134/petStores?PetTypeId={categoryId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var pets = JsonSerializer.Deserialize<List<PetStoreDTO>>(content);
                return View(pets);
            }
            return View(new List<PetStoreDTO>());
        }
    }
}
