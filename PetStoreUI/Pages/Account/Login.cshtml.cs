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
            _logger = logger;
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

            // So sánh mật khẩu đã băm (hash) với mật khẩu người dùng nhập vào
            if (!VerifyPassword(user.Password, Input.Password))
            {
                _logger.LogWarning("Mật khẩu sai cho người dùng: " + Input.Email);
                ViewData["ErrorMessage"] = "Invalid email or password."; // Cung cấp thông báo lỗi cho người dùng
                return Page(); // Quay lại trang login
            }

            // Đăng nhập thành công, tạo claims và đăng nhập người dùng
            _logger.LogInformation("Đăng nhập thành công cho người dùng: " + Input.Email);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var userClaimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userClaimsIdentity));

            // Bảo vệ returnUrl để đảm bảo người dùng không bị redirect ra ngoài ứng dụng
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Content("~/");
            }

            // Redirect về URL mong muốn sau khi đăng nhập thành công
            return LocalRedirect(returnUrl);
        }

        // Phương thức để kiểm tra mật khẩu (mã hóa password)
        private bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            // Sử dụng SHA256 để băm mật khẩu người dùng nhập vào
            using var sha256 = SHA256.Create();
            var inputHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword)));

            // So sánh băm của mật khẩu input với mật khẩu đã băm trong DB
            return inputHash == hashedPassword;
        }
    }
}
