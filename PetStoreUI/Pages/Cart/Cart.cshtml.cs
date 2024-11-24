using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStoreAPI.Data;
using PetStoreAPI.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace PetStoreUI.Pages
{
    public class CartModel : PageModel
    {
        private readonly PetStoreDbContext _dbContext;
        private readonly ILogger<CartModel> _logger;

        public CartModel(PetStoreDbContext dbContext, ILogger<CartModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public List<CartItemViewModel>? CartItems { get; set; }
        public decimal TotalPrice { get; set; }

        public async Task OnGetAsync()
        {
            var emailClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(emailClaim))
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);
                if (user != null)
                {
                    var userEmail = user.Email;

                    CartItems = await _dbContext.CartItems
                        .Include(c => c.Pet)
                        .Where(c => c.UserEmail == userEmail)
                        .GroupBy(c => c.PetId)
                        .Select(g => new CartItemViewModel
                        {
                            CartItemId = g.First().Id,
                            Pet = g.First().Pet,
                            Quantity = g.Sum(c => c.Quantity)
                        })
                        .ToListAsync();

                    TotalPrice = CartItems.Sum(item => item.Pet.Price * item.Quantity);
                }
                else
                {
                    _logger.LogError("User not found for the provided email.");
                    CartItems = new List<CartItemViewModel>();
                    TotalPrice = 0;
                }
            }
            else
            {
                _logger.LogError("Email not found in Claims.");
                CartItems = new List<CartItemViewModel>();
                TotalPrice = 0;
            }
        }

        public async Task<IActionResult> OnPostUpdateCartAsync(int cartItemId, int quantity)
        {
            var cartItem = await _dbContext.CartItems.FirstOrDefaultAsync(ci => ci.Id == cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }

    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public PetStoreEntity? Pet { get; set; }
        public int Quantity { get; set; }
    }
}