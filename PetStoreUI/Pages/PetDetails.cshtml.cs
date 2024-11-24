using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStoreAPI.Data;
using PetStoreLibrary.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PetStoreUI.Pages
{
    public class PetDetailsModel : PageModel
    {
        private readonly PetStoreDbContext _dbContext;

        public PetDetailsModel(PetStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PetStoreDetailDTO? Pet { get; set; }

        public async Task OnGetAsync(int id)
        {
            if (id == 0)
            {
                Pet = null;
                return;
            }

            var petStore = _dbContext.PetStores != null 
                ? await _dbContext.PetStores
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id)
                : null;

            if (petStore != null)
            {
                Pet = new PetStoreDetailDTO(
                    petStore.Id,
                    petStore.PetName,
                    new CategoryDTO(petStore.Category.Id, petStore.Category.Name),
                    petStore.Gender,
                    petStore.PetDescription,
                    petStore.Price,
                    DateOnly.FromDateTime(petStore.BirthDay),
                    petStore.ImageUrl);
            }
            else
            {
                Pet = null;
            }
        }
    }
}