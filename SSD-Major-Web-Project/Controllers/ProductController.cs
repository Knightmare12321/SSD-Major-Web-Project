using Microsoft.AspNetCore.Mvc;
using System.Web;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;
using Newtonsoft.Json;
using EllipticCurve.Utils;

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
                // Set the expired date when cookie is updated/created.
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(365);
                // Retrieve the user's cart from the cookies.
                var cartSession = Request.Cookies[addType];
                // If the cart doesn't exist,
                // create a new cart and add product to the cart.
                if (cartSession == null)
                {
                    List<int> cart = new List<int>();
                    cart.Add(id);
                    string cartJson = JsonConvert.SerializeObject(cart);
                    Response.Cookies.Append(addType, cartJson, option);
                }
                else
                {
                    List<int> cart = JsonConvert.DeserializeObject<List<int>>(cartSession);
                    cart.Add(id);
                    string cartJson = JsonConvert.SerializeObject(cart);
                    Response.Cookies.Append(addType, cartJson, option);
                }

            }

            ProductDetailVM? vm = products.GetByIdVM(id);
            return View(vm);
        }

        public IActionResult Favorite()
        {
            var cartSession = Request.Cookies["cart"];
            List<Product> results = new List<Product>();
            if (cartSession != null)
            {
                List<int> cart = JsonConvert.DeserializeObject<List<int>>(cartSession);
                ProductRepo products = new ProductRepo(_context);
                foreach (var id in cart)
                {
                    results.Add(products.GetById(id));
                }
            }

            return View(results);
            
        }
    }
}
