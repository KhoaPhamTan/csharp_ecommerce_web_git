using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using PetStoreLibrary; // Tham chiếu đến Library
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YourMvcProject.Controllers
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
