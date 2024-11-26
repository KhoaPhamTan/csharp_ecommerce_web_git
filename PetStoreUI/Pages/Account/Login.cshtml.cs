using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace PetStoreUI.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly PetStoreDbContext _dbContext;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(PetStoreDbContext dbContext, ILogger<LoginModel> logger)
        {
            _dbContext = dbContext;
            _logger = _logger;
            Input = new LoginInputModel { Email = string.Empty, Password = string.Empty };
            ReturnUrl = string.Empty;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class LoginInputModel
        {
            [Required(ErrorMessage = "The Email field is required.")]
            [EmailAddress]
            public required string Email { get; set; }

            [Required(ErrorMessage = "The Password field is required.")]
            [DataType(DataType.Password)]
            public required string Password { get; set; }
        }

        // Phương thức GET
        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        // Phương thức POST khi người dùng login
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            // Log the input values
            _logger.LogInformation("OnPostAsync called");
            _logger.LogInformation($"Email: {Input.Email}, Password: {Input.Password}");

            // Kiểm tra ModelState hợp lệ
            if (!ModelState.IsValid)
            {
                // Log lỗi chi tiết về ModelState
                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    var errors = state.Value.Errors;
                    foreach (var error in errors)
                    {
                        _logger.LogWarning($"ModelState error in {key}: {error.ErrorMessage}");
                    }
                }
                // Return lại trang với thông báo lỗi
                return Page();
            }

            // Kiểm tra người dùng trong cơ sở dữ liệu
            var user = _dbContext.Users != null ? await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == Input.Email) : null;
            if (user == null)
            {
                _logger.LogWarning("Không tìm thấy người dùng với email: " + Input.Email);
                ViewData["ErrorMessage"] = "Invalid email or password."; // Cung cấp thông báo lỗi cho người dùng
                return Page(); // Quay lại trang login
            }

            // Đăng nhập thành công, tạo claims và đăng nhập người dùng
            _logger.LogInformation("Đăng nhập thành công cho người dùng: " + Input.Email);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Ensure userId is set as string
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Add role claim
            };

            var userClaimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // Set the persistence of the authentication session
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userClaimsIdentity), authProperties);

            // Log the claims for debugging
            _logger.LogInformation("User logged in with the following claims:");
            foreach (var claim in claims)
            {
                _logger.LogInformation($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            // Bảo vệ returnUrl để đảm bảo người dùng không bị redirect ra ngoài ứng dụng
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Content("~/");
            }

            // Redirect về URL mong muốn sau khi đăng nhập thành công
            return LocalRedirect(returnUrl);
        }
    }
}
