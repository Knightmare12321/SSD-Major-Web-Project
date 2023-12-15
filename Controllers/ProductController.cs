using Microsoft.AspNetCore.Mvc;
using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly NovaDbContext _context;

        public ProductController(NovaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
