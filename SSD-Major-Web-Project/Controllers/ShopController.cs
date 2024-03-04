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
        // Click on the "Proceed to checkout" button at shopping cart page, pass the shopping cart data to checkout view
        [HttpPost]
        public IActionResult Checkout(ShoppingCartVM shoppingcartVM)
        {
            //Create a new Checkout view model object
            CheckoutVM checkoutVM = new CheckoutVM();

            // Assign Discount Code if available from the shopping cart razor view
            Discount discount = new Discount();
            discount.PkDiscountCode = shoppingcartVM.CouponCode != null ? shoppingcartVM.CouponCode : null;

            Contact contact = new Contact();

            // Assign the value from the Razor view to the Order property of the CheckoutVM object
            OrderVM orderVM = new OrderVM
            {
                OrderDate = DateOnly.FromDateTime(DateTime.Today),
                OrderStatus = "Pending",
                Discount = discount,
                Contact = contact,
                OrderTotal = shoppingcartVM.GrandTotal
            };

            checkoutVM.Order = orderVM;

            // Populates the Product(s) include Images property in shopping cart to Checkout view razer page
            List<Product> products = _context.Products.Include(p => p.Images).ToList();


            // Pass the shopping cart products data to Checkout razer view
            shoppingcartVM.Products = products;

            // Pass the shopping cart data from shopping cart vew model to Checkout view model

            // Info for proceed paypal payment
            shoppingcartVM.Currency = "CAD";
            shoppingcartVM.CurrencySymbol = "$";

            checkoutVM.ShoppingCart = shoppingcartVM;    

            
            return View("Checkout", checkoutVM);
        }


        // GET: ShopController/CreateNewOrder (Order Confirmation Page)
        public IActionResult CreateNewOrder(string transactionId, decimal amount, string payerName, CheckoutVM checkoutVM)
        {
          

            // Create an instance of OrderConfirmationVM and populate its properties
            var orderConfirmation = new OrderConfirmationVM
            {
                TransactionId = transactionId,
                Amount = amount,
                PayerName = payerName,
                CheckoutVM = checkoutVM
            };

            // Return the OrderConfirmation view, passing the orderConfirmation object
            return View(orderConfirmation);
        }


        // POST: ShopController/CreateNewOrder
        [HttpPost]
        public IActionResult CreateNewOrder(CheckoutVM checkoutVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Populate the checkoutVM object with the necessary data
                    checkoutVM.Order.OrderStatus = "Pending";
                   // Create order and save to the database, regardless of the transaction's success
                ShopRepo _shopRepo = new ShopRepo(_context);
                string message = _shopRepo.AddOrder(checkoutVM);

                // Log the inserted order details
                _logger.LogInformation("*******************Order inserted into the database. Order ID: {OrderId}, Order Date: {OrderDate}, Order Total: {OrderTotal}********************", checkoutVM.Order.OrderId, checkoutVM.Order.OrderDate, checkoutVM.Order.OrderTotal);

                    // At this point, the order is already created in the database
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inserting order into the database");
                    return View("Error", new ErrorViewModel { RequestId = "Error inserting order into the database" });
                }

                // Now check if the payment is successful
                // if transaction Id is not null, then the payment is successful
                if(checkoutVM.TransactionId != null)
                {
                    // Change the order status to "Paid" and update the order with the transaction ID
                    // Update the order using the _shopRepo or your preferred method

                    // Compare and get the transaction ID from PayPal, update the order with the transaction ID,
                    // make a request to PayPal using the transaction ID to get order details from PayPal,
                    // and compare them with the order details we received in this method

                    OrderConfirmationVM orderconfirmationVM = new OrderConfirmationVM();
                    // Populate the orderconfirmationVM with the necessary data

                    return View("OrderConfirmation");
                }
                else
                {
                    // Return the Checkout page with the checkout view model again
                    // return View("Checkout", checkoutVM);
                    return View("Checkout");
                }
            }
            return View("Error");
        }


    }
}
