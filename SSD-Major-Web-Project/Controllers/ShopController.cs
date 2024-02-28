using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;
using System;
using System.Net;

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
            try
            {
                // catch error, if no products found, show error message " There is no product available now, please come back later"
                if (_context.Products.Count() == 0)
                {
                    return View("Error", new ErrorViewModel { RequestId = "Your shopping cart is empty" });
                }
                else
                {
                    //populates the Images property of each Product object
                    List<Product> products = _context.Products.Include(p => p.Images).ToList();

                    // assign products to shoppingcartVM using a product array hard coded
                    //List<Product> products = new List<Product>
                    //{
                    //    new Product { PkProductId = 1, Name = "Product 1", Price = 100 },
                    //    new Product { PkProductId = 2, Name = "Product 2", Price = 200 },
                    //    new Product { PkProductId = 3, Name = "Product 3", Price = 300 }
                    //};

                    ShoppingCartVM shoppingcartVM = new ShoppingCartVM();
                    //shoppingcart.UserId = "user123";
                    ShopRepo _shopRepo = new ShopRepo(_context);
                    shoppingcartVM.Subtotal = _shopRepo.CalculateSubtotal(products);
                    shoppingcartVM.ShippingFee = 0;
                    shoppingcartVM.Taxes = _shopRepo.CalculateTaxes(shoppingcartVM.Subtotal);
                    shoppingcartVM.GrandTotal = _shopRepo.CalculateGrandTotal(shoppingcartVM.Subtotal, shoppingcartVM.Taxes, shoppingcartVM.ShippingFee);

                    shoppingcartVM.Products = products;

                    return View(shoppingcartVM);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ShopController");
                return View("Error", new ErrorViewModel { RequestId = "Your shopping cart is empty" });
            }
        }

        // GET: ShopController/Checkout
        [HttpPost]
        public IActionResult Checkout(CheckoutVM vm)
        {
            //List<Product> products = _context.Products.ToList();
            List<Product> products = new List<Product>
            {
                new Product { PkProductId = 1, Name = "Product 1", Price = 100 },
                new Product { PkProductId = 2, Name = "Product 2", Price = 200 },
                new Product { PkProductId = 3, Name = "Product 3", Price = 300 }
            };

            ShoppingCartVM shoppingcartVM = new ShoppingCartVM();

            ShopRepo _shopRepo = new ShopRepo(_context);
            shoppingcartVM.Subtotal = _shopRepo.CalculateSubtotal(products);
            shoppingcartVM.ShippingFee = 0;
            shoppingcartVM.Taxes = _shopRepo.CalculateTaxes(shoppingcartVM.Subtotal);
            shoppingcartVM.GrandTotal = _shopRepo.CalculateGrandTotal(shoppingcartVM.Subtotal, shoppingcartVM.Taxes, shoppingcartVM.ShippingFee);
            shoppingcartVM.Currency = "CAD";
            shoppingcartVM.CurrencySymbol = "$";
            shoppingcartVM.Products = products;

            //pass the shoppingcart data to Checkout view model
            vm.ShoppingCart = shoppingcartVM;

            // if user is not logged in, ask user to enter address
            //shoppingcartVM.Order.Contact.FirstName = FirstName;
            //shoppingcartVM.Order.Contact.LastName = LastName;
            //shoppingcartVM.Order.Contact.Address = address;
            //shoppingcartVM.Order.Contact.Address2 = address2;
            //shoppingcartVM.Order.Contact.City = city;
            //shoppingcartVM.Order.Contact.province = province;
            //shoppingcartVM.Order.Contact.Country = country;
            //shoppingcartVM.Order.Contact.PostalCode = postalCode;
            //shoppingcartVM.Order.Contact.Phone = phone;

            // if user logged in, display user's default adress as shipping option
            return View("Checkout", vm);
        }


        public IActionResult OrderConfirmation(OrderConfirmationVM orderConfirmationModel)
        {
            return View(orderConfirmationModel);
        }





        //// GET: ShopController/CreateOrder
        //public IActionResult CreateOrder()
        //{
        //    return View();
        //}

        // POST: ShopController/CreateOrder
        //[HttpPost]
        //public async Task<IActionResult> CreateOrder(Order order)
        //{
        //    // check if data valid
        //    if (ModelState.IsValid)
        //    {
        //        // create order
        //        order = new Order();
        //        order.OrderDate = DateOnly;
        //        order.FkAddress = vm.ShippingAddress;
        //        order.ShippingCity = vm.ShippingCity;
        //        order.ShippingCountry = vm.ShippingCountry;
        //        order.ShippingPostalCode = vm.ShippingPostalCode;
        //        order.ShippingState = vm.ShippingState;
        //        order.UserId = "user123";
        //        order.OrderItems = new List<OrderItem>();

        //        // create order items
        //        foreach (Product product in vm.Products)
        //        {
        //            OrderItem orderItem = new OrderItem();
        //            orderItem.ProductId = product.Id;
        //            orderItem.Quantity = product.Quantity;
        //            orderItem.Price = product.Price;
        //            order.OrderItems.Add(orderItem);
        //        }

        //        // save order to database
        //        _context.Orders.Add(order);
        //        await _context.SaveChangesAsync();

        //        // redirect to order confirmation page
        //        return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        //    }
        //    else
        //    {
        //        return View(vm);
        //    }


        //}


    }
}
