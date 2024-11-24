using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetStoreUI.Pages
{
    public class CartModel : PageModel
    {
        private readonly PetStoreDbContext _dbContext;

        public CartModel(PetStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CartItemViewModel> CartItems { get; set; }
        public decimal TotalPrice { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null && int.TryParse(userId, out int parsedUserId))
            {
                CartItems = await _dbContext.CartItems
                    .Include(c => c.Pet)
                    .Where(c => c.UserId == parsedUserId)
                    .GroupBy(c => c.PetId)
                    .Select(g => new CartItemViewModel
                    {
                        Pet = g.First().Pet,
                        Quantity = g.Sum(c => c.Quantity)
                    })
                    .ToListAsync();

                TotalPrice = CartItems.Sum(item => item.Pet.Price * item.Quantity);
            }
            else
            {
                CartItems = new List<CartItemViewModel>();
                TotalPrice = 0;
            }
        }
    }

    public class CartItemViewModel
    {
        public PetStoreEntity Pet { get; set; }
        public int Quantity { get; set; }
    }
}