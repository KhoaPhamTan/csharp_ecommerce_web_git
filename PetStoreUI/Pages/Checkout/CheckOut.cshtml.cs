using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetStoreAPI.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetStoreUI.Pages.Checkout
{
    public class CheckoutModel : PageModel
    {
        private readonly PetStoreDbContext _dbContext;
        private readonly ILogger<CheckoutModel> _logger;

        public CheckoutModel(PetStoreDbContext dbContext, ILogger<CheckoutModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public async Task OnGetAsync()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(emailClaim))
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);
                if (user != null)
                {
                    FullName = user.FullName;
                    Email = user.Email;
                    Address = user.Address;
                }
                _logger.LogInformation("User details loaded for email: {Email}", emailClaim);
            }
        }
    }
}
