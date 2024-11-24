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

            PetStores = _dbContext.PetStores
                .Include(p => p.Category)
                .Where(p => p.Category.Name == categoryName)
                .Select(p => new PetStoreDTO(p.Id, p.PetName, p.Category.Name, p.Gender, p.PetDescription, p.Price, DateOnly.FromDateTime(p.BirthDay), p.ImageUrl))
                .ToList();

            Categories = _dbContext.Categories
                .Select(c => new CategoryDTO(c.Id, c.Name))
                .ToList();
        }
    }
}