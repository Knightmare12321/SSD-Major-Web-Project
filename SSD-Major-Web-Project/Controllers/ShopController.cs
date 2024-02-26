using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;   
using SSD_Major_Web_Project.ViewModels;
using System;

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
            List<Product> products = _context.Products.ToList();


            ShoppingCartVM shoppingcartVM = new ShoppingCartVM();

            //shoppingcart.UserId = "user123";
            //for each product in product, assign ImageByteArray to product.Image
            ShopRepository _shopRepo = new ShopRepository(_context);
            shoppingcartVM.Subtotal = _shopRepo.CalculateSubtotal(products);
            shoppingcartVM.ShippingFee = 0;
            shoppingcartVM.Taxes = _shopRepo.CalculateTaxes(shoppingcartVM.Subtotal) ;
            shoppingcartVM.GrandTotal = _shopRepo.CalculateGrandTotal(shoppingcartVM.Subtotal, shoppingcartVM.Taxes, shoppingcartVM.ShippingFee);

            shoppingcartVM.Products = products;

            return View(shoppingcartVM);
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
