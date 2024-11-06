using Microsoft.AspNetCore.Mvc;
using PetStoreUI.Services;
using PetStoreLibrary.DTOs;
using System.Collections.Generic;
using System.Diagnostics; // Thêm chỉ thị này nếu chưa có
using PetStoreUI.Models; // Thêm chỉ thị này nếu chưa có

namespace PetStoreUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPetService _petService;

        public HomeController(IPetService petService)
        {
            _petService = petService;
        }

        public IActionResult Index()
        {
            try
            {
                List<PetTypeDTO> petTypes = _petService.GetAllPetTypes();
                return View(petTypes);
            }
            catch (Exception)
            {
                // Xử lý ngoại lệ mà không cần sử dụng biến ex
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}