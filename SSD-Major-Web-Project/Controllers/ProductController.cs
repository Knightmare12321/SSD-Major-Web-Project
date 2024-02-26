using Microsoft.AspNetCore.Mvc;
using System.Web;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;
using Newtonsoft.Json;

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

        public IActionResult Details(int id, string addType)
        {
            ProductRepo products = new ProductRepo(_context);
            if (addType == "cart" || addType == "favorite")
            {
                // Retrieve the user's cart from the session
                var cartSession = HttpContext.Session.GetString(addType);
                string cartJson;
                // If the cart session variable doesn't exist,
                // create a new cart and add product to the cart.
                if (cartSession == null)
                {
                    Cart cart = new Cart();
                    cart.AddItem(products.GetById(id));
                    cartJson = JsonConvert.SerializeObject(cart);
                    HttpContext.Session.SetString(addType, cartJson);
                }
                else
                {
                    Cart cart = JsonConvert.DeserializeObject<Cart>(cartSession);
                    cart.AddItem(products.GetById(id));
                    cartJson = JsonConvert.SerializeObject(cart);
                    HttpContext.Session.SetString(addType, cartJson);
                }

            }

            ProductDetailVM? vm = products.GetByIdVM(id);
            return View(vm);
        }

        public IActionResult Favorite()
        {
            var cartSession = HttpContext.Session.GetString("cart");
            if (cartSession != null)
            {
                Cart cart = JsonConvert.DeserializeObject<Cart>(cartSession);
                return View(cart.Items);
            }

            return View();
            
        }
    }
}
