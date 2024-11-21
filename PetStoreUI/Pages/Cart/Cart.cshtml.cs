using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStoreLibrary.DTOs;
using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PetStoreUI.Pages.Cart
{
    public class CartModel : PageModel
    {
        private readonly PetStoreDbContext _dbContext;

        public CartModel(PetStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                CartItems = new List<CartItemDTO>();
                return;
            }

            // Fetch cart items from the database for the logged-in user
            var cartItems = await _dbContext.CartItems
                .Include(ci => ci.Pet)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            CartItems = cartItems.Select(ci => new CartItemDTO(ci.PetId, ci.Quantity)).ToList();
        }
    }
}