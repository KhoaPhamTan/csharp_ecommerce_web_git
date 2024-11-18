using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace PetStoreUI.Pages
{
    public class ErrorModel : PageModel
    {
        // ...existing code...
        public string RequestId { get; set; } = string.Empty;

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
        // ...existing code...
    }
}
