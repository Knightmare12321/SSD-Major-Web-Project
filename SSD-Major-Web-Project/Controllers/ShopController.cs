using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                var shoppingCartCookie = Request.Cookies["cart"];
                List<int> IDList = JsonConvert.DeserializeObject<List<int>>(shoppingCartCookie);


                //Catch error, if shopping cart is empty(no products found), show a message"
                if (IDList.Count() == 0)
                {
                    return View("Error", new ErrorViewModel { RequestId = "Your shopping cart is empty. Shop now"});
                }
                else
                {

          

                    List<ShoppingCartItem> shoppingcartItems = new List<ShoppingCartItem>();
                    for (int i = 0; i < IDList.Count; i++)
                    {
                        shoppingcartItems.Add(new ShoppingCartItem { SkuId = IDList[i], Quantity = 1 });
                    }

                    //productIdsFromDb list contains the ProductId values associated with the provided SkuIds from the database.
                    List<int> skuIds = shoppingcartItems.Select(s => s.SkuId).ToList();

                    List<ProductSku> productSkus = _context.ProductSkus
                        .Where(p => skuIds.Contains(p.PkSkuId))
                        .ToList();

                    List<int> productIds = productSkus.Select(p => p.FkProductId ?? 0).ToList();

                    List<Product> products = _context.Products
                     .Include(p => p.Images)
                     .Where(p => productIds.Contains(p.PkProductId))
                     .ToList();

                    //populates the Product(s) by skuId include Images property in shopping cart for shopping cart view


                    ShoppingCartVM shoppingcartVM = new ShoppingCartVM();

                    shoppingcartVM.ShoppingCartItems = shoppingcartItems;

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




        // POST: ShopController/Checkout
        // Click on the "Proceed to checkout" button at shopping cart page, pass the shopping cart data to Checkout view
        [HttpPost]
        public IActionResult CheckoutShippingContact(ShoppingCartVM shoppingcartVM)
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

            // geting shopping cart data from cookies
            var shoppingCartCookie = Request.Cookies["cart"];
            List<int> IDList = JsonConvert.DeserializeObject<List<int>>(shoppingCartCookie);

            List<ShoppingCartItem> shoppingcartItems = new List<ShoppingCartItem>();
            for (int i = 0; i < IDList.Count; i++)
            {
                shoppingcartItems.Add(new ShoppingCartItem { SkuId = IDList[i], Quantity = 1 });
            }

            //productIdsFromDb list contains the ProductId values associated with the provided SkuIds from the database.
            List<int> skuIds = shoppingcartItems.Select(s => s.SkuId).ToList();

            List<ProductSku> productSkus = _context.ProductSkus
                .Where(p => skuIds.Contains(p.PkSkuId))
                .ToList();

            List<int> productIds = productSkus.Select(p => p.FkProductId ?? 0).ToList();

            List<Product> products = _context.Products
             .Include(p => p.Images)
             .Where(p => productIds.Contains(p.PkProductId))
             .ToList();

            shoppingcartVM.ShoppingCartItems = shoppingcartItems;


            // Pass the shopping cart products data to Checkout razer view
            shoppingcartVM.Products = products;

            // Pass the shopping cart data from shopping cart vew model to Checkout view model

            // Info for proceed paypal payment
            shoppingcartVM.Currency = "CAD";
            shoppingcartVM.CurrencySymbol = "$";

            checkoutVM.ShoppingCart = shoppingcartVM;    
            checkoutVM.ShoppingCart.ShoppingCartItems = shoppingcartVM.ShoppingCartItems;

            
            return View("CheckoutShippingContact", checkoutVM);
        }

        // POST: ShopController/Checkout
        // Click on the "Proceed to Payment Page" button at Enter Shipping Contact page, create order to db at this stage
        [HttpPost]
        public IActionResult ProceedPayment(CheckoutVM checkoutVM)
        {
            ShopRepo _shopRepo = new ShopRepo(_context);

            // Assign Discount Code if available from the shopping cart razor view
            Discount discount = new Discount();
            
            discount.PkDiscountCode = checkoutVM.ShoppingCart.CouponCode != null ? checkoutVM.ShoppingCart.CouponCode : null;

            Contact contact = new Contact();

            contact.FirstName = checkoutVM.Order.Contact.FirstName;
            contact.LastName = checkoutVM.Order.Contact.LastName;
            contact.Address = checkoutVM.Order.Contact.Address;
            contact.Address2 = checkoutVM.Order.Contact.Address2;
            contact.City = checkoutVM.Order.Contact.City;
            contact.Province = checkoutVM.Order.Contact.Province;
            contact.Country = checkoutVM.Order.Contact.Country;
            contact.PostalCode = checkoutVM.Order.Contact.PostalCode;
            contact.PhoneNumber = checkoutVM.Order.Contact.PhoneNumber;
        
            

            // Assign the value from the Razor view to the Order property of the CheckoutVM object
            OrderVM orderVM = new OrderVM
            {
                OrderDate = DateOnly.FromDateTime(DateTime.Today),
                OrderStatus = "Pending",
                BuyerNote = checkoutVM.Order.BuyerNote,
                Discount = discount,
                Contact = contact,
                OrderTotal = checkoutVM.ShoppingCart.GrandTotal
            };

            //Order Detail
            OrderDetail orderDetail = new OrderDetail();

           
            ShoppingCartItem shoppingCartItem = new ShoppingCartItem();


            // for loop to get the product details from the shopping cart
            foreach (var product in checkoutVM.ShoppingCart.ShoppingCartItems)
            {
                orderDetail.FkSkuId = product.SkuId;
                orderDetail.Quantity = product.Quantity;
           

                // Retrieve the unit price from the database based on the SKU ID
                var productSku = _context.ProductSkus.FirstOrDefault(p => p.PkSkuId == product.SkuId);
                if (productSku != null)
                {
                    // Assign the retrieved unit price to orderDetail.UnitPrice
                    orderDetail.UnitPrice = productSku.FkProduct?.Price ?? 0;
                }
                else
                {
                    // Handle the case when the product SKU is not found
                    // You can set a default value or handle the exception accordingly
                    orderDetail.UnitPrice = 0;
                }

              
            }


            // userId is nullable
            if (User.Identity.IsAuthenticated)
            {
                checkoutVM.ShoppingCart.UserId = User.Identity.Name;
            }
            else
            {
                checkoutVM.ShoppingCart.UserId = null;
            }


            // Pass the shopping cart products data to Checkout razer view
            var shoppingCartCookie = Request.Cookies["cart"];
            List<int> IDList = JsonConvert.DeserializeObject<List<int>>(shoppingCartCookie);

            List<ShoppingCartItem> shoppingcartItems = new List<ShoppingCartItem>();
            for (int i = 0; i < IDList.Count; i++)
            {
                shoppingcartItems.Add(new ShoppingCartItem { SkuId = IDList[i], Quantity = 1 });
            }

            //productIdsFromDb list contains the ProductId values associated with the provided SkuIds from the database.
            List<int> skuIds = shoppingcartItems.Select(s => s.SkuId).ToList();

            List<ProductSku> productSkus = _context.ProductSkus
                .Where(p => skuIds.Contains(p.PkSkuId))
                .ToList();

            List<int> productIds = productSkus.Select(p => p.FkProductId ?? 0).ToList();

            List<Product> products = _context.Products
             .Include(p => p.Images)
             .Where(p => productIds.Contains(p.PkProductId))
             .ToList();

            checkoutVM.ShoppingCart.ShoppingCartItems = shoppingcartItems;

            checkoutVM.ShoppingCart.Products = products;

            // initialize the list of order details
            List<OrderDetail> listOfOrderDetails = new List<OrderDetail>();
            
            // for each product in the shopping cart, create an order detail and add it to the list of order details
            foreach (var product in checkoutVM.ShoppingCart.ShoppingCartItems)
            {
                OrderDetail singleOrderDetailRecord = new OrderDetail
                {
                    FkSkuId = product.SkuId,
                    Quantity = product.Quantity
                };

                // Retrieve the unit price from the database based on the SKU ID
                var productSku = _context.ProductSkus.FirstOrDefault(p => p.PkSkuId == product.SkuId);
                if (productSku != null)
                {
                    // Assign the retrieved unit price to orderDetail.UnitPrice
                    singleOrderDetailRecord.UnitPrice = productSku.FkProduct?.Price ?? 0;
                }
                else
                {
                    // Handle the case when the product SKU is not found
                    // You can set a default value or handle the exception accordingly
                    singleOrderDetailRecord.UnitPrice = 0;
                }

                listOfOrderDetails.Add(singleOrderDetailRecord);
            }

            checkoutVM.Order.OrderDetails = listOfOrderDetails;

 
            OrderConfirmationVM orderConfirmationVM = new OrderConfirmationVM();

            orderConfirmationVM.CheckoutVM = checkoutVM;


            // Add the contact to the database
            var addContactResult = _shopRepo.AddContact(checkoutVM.Order.Contact);
            string message = addContactResult.Item1;
            int contactId = addContactResult.Item2;

            //if (string.IsNullOrEmpty(message))
            //{
            //    ViewBag.Message = $"Contact ID - {contactId} has been added.";
            //    return View("CheckoutShippingContact", checkoutVM);
            //}
            //else
            //{
            //    ViewBag.Message = $"Error creating new contact: {message}";
            //    return View("CheckoutShippingContact", checkoutVM);
            //}


            Tuple<string, int> result = _shopRepo.AddOrder(checkoutVM, contactId);

            string errorAddOrder = result.Item1;
            int orderId = result.Item2;

            // add order details
            string errorAddOrderDetails = _shopRepo.AddOrderDetails(checkoutVM, orderId);
            

            //string message;
            if (errorAddOrder == "")
            {
                message = $"Order {checkoutVM.Order.OrderId} has been placed, you will get an email confirmation shortly once prcoeed payment.";
                return View("Paypal", checkoutVM);
            }
            else
            {
                message = $"Error placing new order: {checkoutVM.Order.OrderId}";
                ViewBag.Message = message;
                return View("CheckoutShippingContact", checkoutVM);
            }

            //// Add the Order Detail to the database
            //var addOrderDetailResult = _shopRepo.AddOrderDetails(checkoutVM.Order.OrderDetails);


        }

        // GET: ShopController//orderConfirmation
        public IActionResult OrderConfirmation(string transactionId, decimal amount, string payerName, CheckoutVM checkoutVM)
        {
            
            // Create an instance of OrderConfirmationVM and populate its properties
            var orderConfirmation = new OrderConfirmationVM
            {
                TransactionId = transactionId,
                Amount = amount,
                PayerName = payerName,
                CheckoutVM = checkoutVM
            };

            OrderConfirmationVM orderConfirmationVM = new OrderConfirmationVM();
            orderConfirmationVM.CheckoutVM = checkoutVM;

            if (orderConfirmationVM.CheckoutVM.TransactionId != null)
                    {         
                        // At this point, the order is already created in the database
                        // Change the order status to "Paid" and update the order with the transaction ID
                        
                        

                        // Compare and get the transaction ID from PayPal, update the order with the transaction ID,
                        // make a request to PayPal using the transaction ID to get order details from PayPal,
                        // and compare them with the order details we received in this method

                        OrderConfirmationVM orderconfirmationVM = new OrderConfirmationVM();
                        // Populate the orderconfirmationVM with the necessary data

                        return View("OrderConfirmation",orderConfirmation);
                    }
                    else
                    {
                        // Return the Checkout page with the checkout view model again
                        return View("CheckoutShippingContact", checkoutVM);
                    }
                
              
            }

       

    }
}
