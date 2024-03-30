using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;

namespace SSD_Major_Web_Project.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly NovaDbContext _context;

        public FavoriteController(NovaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var favoriteCookie = Request.Cookies["favorite"];
            var favoriteIDs = JsonConvert.DeserializeObject<List<int>>(favoriteCookie);
            ProductRepo products = new ProductRepo(_context);
            return View(products.GetProductByIdList(favoriteIDs));
        }
    }
}
