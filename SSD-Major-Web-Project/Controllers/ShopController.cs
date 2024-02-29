using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;
using System;
using System.Diagnostics.Metrics;
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
                //Catch error, if shopping cart is empty(no products found), show a message"
                if (_context.Products.Count() == 0)
                {
                    return View("Error", new ErrorViewModel { RequestId = "Your shopping cart is empty. Shop now"});
                }
                else
                {
                    //populates the Product(s) include Images property in shopping cart for shopping cart view
                    List<Product> products = _context.Products.Include(p => p.Images).ToList();

                    ShoppingCartVM shoppingcartVM = new ShoppingCartVM();

//////////////////////Logic for validate if customer logged in
                    //if (User.Identity.IsAuthenticated)
                    //{
                    //    shoppingcart.UserId = User.Identity.Name;
                    //}

                    //Use repo helper function to calculate subtotal, taxes, shipping fee and grand total
                    ShopRepo _shopRepo = new ShopRepo(_context);
                    shoppingcartVM.Subtotal = _shopRepo.CalculateSubtotal(products);
                    shoppingcartVM.ShippingFee = 0; // shipping fee is 0 for now
                    shoppingcartVM.Taxes = _shopRepo.CalculateTaxes(shoppingcartVM.Subtotal);
                    shoppingcartVM.GrandTotal = _shopRepo.CalculateGrandTotal(shoppingcartVM.Subtotal, shoppingcartVM.Taxes, shoppingcartVM.ShippingFee);

//////////////////////Assign shopping cart products to shopping cart view model
                    shoppingcartVM.Products = products;

                    return View(shoppingcartVM);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ShopController");
                return View("Error", new ErrorViewModel { RequestId = "Error in ShopController" });
            }
        }

        // GET: ShopController/Checkout
        [HttpPost]
        //Click on the "Proceed to checkout" button, pass the shopping cart data to checkout view
        public IActionResult Checkout(CheckoutVM checkoutVM)
        {
            //Populates the Product(s) include Images property in shopping cart to Checkout view razer page
            List<Product> products = _context.Products.Include(p => p.Images).ToList();

            //Pass the shoppingcart products data to Checkout view razer page
            ShoppingCartVM shoppingcartVM = new ShoppingCartVM();
            shoppingcartVM.Products = products;

            
            ShopRepo _shopRepo = new ShopRepo(_context);
            shoppingcartVM.Subtotal = _shopRepo.CalculateSubtotal(products);
            shoppingcartVM.ShippingFee = 0;
            shoppingcartVM.Taxes = _shopRepo.CalculateTaxes(shoppingcartVM.Subtotal);
            shoppingcartVM.GrandTotal = _shopRepo.CalculateGrandTotal(shoppingcartVM.Subtotal, shoppingcartVM.Taxes, shoppingcartVM.ShippingFee);

            //Info for proceed paypal payment
            shoppingcartVM.Currency = "CAD";
            shoppingcartVM.CurrencySymbol = "$";


            //pass the shoppingcart data to Checkout view model
            checkoutVM.ShoppingCart = shoppingcartVM;


            return View("Checkout", checkoutVM);
        }


        public IActionResult OrderConfirmation(OrderConfirmationVM orderConfirmationModel)
        {
            return View(orderConfirmationModel);
        }

        [HttpPost]
        public IActionResult OrderConfirmation(string itemPrice, string firstName, string lastName)
        {

            // Pass the values to the view
            ViewBag.ItemPrice = itemPrice;
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;

            return View();
        }


       //  GET: ShopController/CreateOrder
       // public IActionResult CreateOrder()
       // {
       //     Show input filed to enter new address
       //     checkoutVM.Order.Contact.FirstName = FirstName;
       //     checkoutVM.Order.Contact.LastName = LastName;
       //     checkoutVM.Order.Contact.Address = address;
       //     checkoutVM.Order.Contact.Address2 = address2;
       //     checkoutVM.Order.Contact.City = city;
       //     checkoutVM.Order.Contact.Province = province;
       //     checkoutVM.Order.Contact.Country = country;
       //     checkoutVM.Order.Contact.PostalCode = postalCode;
       //     checkoutVM.Order.Contact.PhoneNumber = phone;

       //      if user logged in, display user's default adress as shipping option, use radio button to select

       //     return View();
       // }

       // POST: ShopController/CreateOrder
       //[HttpPost]
       // public async Task<IActionResult> CreateOrder(Order order, Customer customer, Contact contact, Product product, OrderDetail orderDetail, Discount discount)
       // {
       //      check if data valid
       //     if (ModelState.IsValid)
       //     {

       //          create order
       //         order = new Order();
       //         order.OrderDate = DateOnly;
       //         order.FkAddress = vm.ShippingAddress;
       //         order.ShippingCity = vm.ShippingCity;
       //         order.ShippingCountry = vm.ShippingCountry;
       //         order.ShippingPostalCode = vm.ShippingPostalCode;
       //         order.ShippingState = vm.ShippingState;
       //         order.UserId = "user123";
       //         order.OrderItems = new List<OrderItem>();

       //          create order items
       //         foreach (Product product in vm.Products)
       //         {
       //             OrderItem orderItem = new OrderItem();
       //             orderItem.ProductId = product.Id;
       //             orderItem.Quantity = product.Quantity;
       //             orderItem.Price = product.Price;
       //             order.OrderItems.Add(orderItem);
       //         }

       //          save order to database
       //         _context.Orders.Add(order);
       //         await _context.SaveChangesAsync();

       //          redirect to order confirmation page
       //         return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
       //     }
       //     else
       //     {
       //         return View(vm);
       //     }

       // }


    }
}
