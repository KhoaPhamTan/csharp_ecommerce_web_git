using Microsoft.AspNetCore.Mvc;

public class HelpController : Controller
{
    public IActionResult Index()
    {
        return View("Help");
    }
}
