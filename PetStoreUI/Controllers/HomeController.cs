using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;
using PetStoreUI.Models;
using System.Collections.Generic;
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

        public async Task<IActionResult> Index(string categoryName = "Cat")
        {
            ViewData["ActivePage"] = "Home"; // Set active page

            // Get list of Categories from the database
            var categories = await _context.Categories.ToListAsync();

            // Get list of PetStores based on selected category
            var petStores = await _context.PetStores
                .Include(ps => ps.Category)
                .Where(ps => ps.Category.Name == categoryName)
                .ToListAsync();

            var categoryDTOs = categories.Select(c => new CategoryDTO(c.Id, c.Name)).ToList();

            var petStoreDTOs = petStores.Select(ps => new PetStoreDTO(
                ps.Id, 
                ps.PetName, 
                ps.Category.Name, 
                ps.Gender, 
                ps.PetDescription, 
                ps.Price, 
                DateOnly.FromDateTime(ps.BirthDay), 
                ps.ImageUrl)).ToList();

            var viewModel = new PetStoreViewModel
            {
                Categories = categoryDTOs,
                Pets = string.IsNullOrEmpty(categoryName) ? petStoreDTOs : petStoreDTOs.Where(p => p.CategoryName == categoryName).ToList(), // Use CategoryName
                SelectedCategory = categoryName
            };

            return View(viewModel); // Return view with list of categories and pet stores
        }

        public IActionResult Help()
        {
            ViewData["ActivePage"] = "Help"; // Set active page
            return View();
        }
    }
}