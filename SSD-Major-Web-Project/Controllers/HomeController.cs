using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;
using System.Diagnostics;

namespace SSD_Major_Web_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NovaDbContext _context;

        public HomeController(ILogger<HomeController> logger, NovaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            DbSet<Product> products = _context.Products;
            return View(products);
        }

        public string GetUserName()
        {
            string userId = User.Identity.Name;
            string userName = _context.Customers
                .Where(c => c.PkCustomerId == userId)
                .Include(c => c.FkContact)
                .Select(c => c.FkContact.FirstName).FirstOrDefault();
            return userName ?? "";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
