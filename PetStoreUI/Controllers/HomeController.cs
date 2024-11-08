using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;
using PetStoreUI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PetStoreUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly PetStoreDbContext _context;

        public HomeController(PetStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string category = "Cat")
        {
            ViewData["ActivePage"] = "Home"; // Set active page

            // Get list of Categories from the database
            var categories = await _context.Categories.ToListAsync(); // Use ToListAsync

            var categoryDTOs = categories.Select(c => new CategoryDTO(c.Id, c.Name)).ToList();

            var viewModel = new PetStoreViewModel
            {
                Categories = categoryDTOs
            };

            return View(viewModel); // Return view with list of categories
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