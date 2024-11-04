using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Entities; // Namespace cho PetTypeEntity
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PetStoreMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly PetStoreDbContext _context; // DbContext

        public HomeController(HttpClient httpClient, PetStoreDbContext context)
        {
            _httpClient = httpClient;
            _context = context; // Khởi tạo DbContext
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Home"; // Set active page

            // Lấy danh sách Pet Types từ cơ sở dữ liệu
            var petTypes = await _context.PetTypes.ToListAsync(); // Sử dụng ToListAsync

            return View(petTypes); // Trả về view với danh sách pet types
        }

        public IActionResult Privacy()
        {
            ViewData["ActivePage"] = "Privacy"; // Set active page
            return View();
        }

        public IActionResult Help()
        {
            ViewData["ActivePage"] = "Help"; // Set active page
            return View();
        }
    }
}
