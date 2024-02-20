using Microsoft.AspNetCore.Mvc;
using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.Controllers
{
    public class ShopController : Controller
    {

        private readonly ILogger<ShopController> _logger;
        private readonly NovaDbContext _context;

        public ShopController(ILogger<ShopController> logger, NovaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            //List<Product> products = _context.Products.ToList();
            //var shoppingcart = new ShoppingCartVM();

            //shoppingcart.Products = products;
            //shoppingcart.UserId = "user123";
            //shoppingcart.CouponCode = "";
            //shoppingcart.Subtotal = 0;
            //shoppingcart.ShippingFee = 0;
            //shoppingcart.Taxes = 0;
            //shoppingcart.GrandTotal = 0;

            return View();
        }

        // GET: HomeController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}
