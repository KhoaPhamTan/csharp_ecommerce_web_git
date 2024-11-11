using Microsoft.AspNetCore.Mvc;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PetStoreUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly PetStoreDbContext _dbContext;

        // Constructor với dependency injection để lấy DbContext
        public HomeController(PetStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(string categoryName)
        {
            // Set default category to "Cat" if categoryName is null or empty
            if (string.IsNullOrEmpty(categoryName))
            {
                categoryName = "Cat";
            }

            // Set the selected category in ViewBag
            ViewBag.SelectedCategory = categoryName;

            // Lấy danh sách cửa hàng thú cưng theo categoryName
            var petStores = _dbContext.PetStores
                .Include(p => p.Category) // Ensure Category is included
                .Where(p => p.Category.Name == categoryName)
                .Select(p => new PetStoreDTO(p.Id, p.PetName, p.Category.Name, p.Gender, p.PetDescription, p.Price, DateOnly.FromDateTime(p.BirthDay), p.ImageUrl))
                .ToList();

            // Đưa danh sách petStores vào ViewBag
            ViewBag.PetStores = petStores;

            // Lấy danh sách các category và đưa vào ViewBag
            var categories = _dbContext.Categories
                .Select(c => new CategoryDTO(c.Id, c.Name))
                .ToList();
            ViewBag.Categories = categories;

            // Trả về view mà không cần trả về model
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}