using Microsoft.AspNetCore.Mvc;

namespace SSD_Major_Web_Project.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
