using Microsoft.AspNetCore.Mvc;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NovaDbContext _context;

        public ProductController(ILogger<HomeController> logger, NovaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ProductRepo temp = new ProductRepo(_context);
            return View(temp.GetAll());
        }

        public IActionResult Details(int id)
        {
            ProductRepo temp = new ProductRepo(_context);
            ProductDetailVM? vm = temp.GetById(id);
            return View(vm);
        }
    }
}
