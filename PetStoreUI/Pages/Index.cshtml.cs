using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace PetStoreUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PetStoreDbContext _dbContext;

        public IndexModel(PetStoreDbContext dbContext)
        {
            _dbContext = dbContext;
            PetStores = new List<PetStoreDTO>();
            Categories = new List<CategoryDTO>();
            SelectedCategory = string.Empty;
            UserName = string.Empty;
        }

        public List<PetStoreDTO> PetStores { get; set; }
        public List<CategoryDTO> Categories { get; set; }
        public string SelectedCategory { get; set; }
        public string UserName { get; set; }

        public void OnGet(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                categoryName = "Cat";
            }

            SelectedCategory = categoryName;

            var petStoresQuery = _dbContext.PetStores.AsQueryable();
            if (petStoresQuery != null)
            {
                PetStores = petStoresQuery.Include(p => p.Category)
                    .Where(p => p.Category.Name == categoryName)
                    .Select(p => new PetStoreDTO(p.Id, p.PetName, p.Category.Name, p.Gender, p.PetDescription, p.Price, DateOnly.FromDateTime(p.BirthDay), Url.Content("~/images/" + p.ImageUrl))) // Ensure ImageUrl is correctly set
                    .ToList();
            }

            var categoriesQuery = _dbContext.Categories.AsQueryable();
            if (categoriesQuery != null)
            {
                Categories = categoriesQuery.Select(c => new CategoryDTO(c.Id, c.Name)).ToList();
            }
        }
    }
}