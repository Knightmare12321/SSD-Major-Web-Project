using Microsoft.AspNetCore.Mvc;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateProduct()
        {
            CreateProductVM vm = new CreateProductVM();
            return View(vm);
        }

    }
}
