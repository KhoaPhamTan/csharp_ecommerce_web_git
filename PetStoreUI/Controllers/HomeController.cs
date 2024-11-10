using Microsoft.AspNetCore.Mvc;

namespace PetStoreUI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string categoryName)
        {
            // Logic to handle categoryName if needed
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
