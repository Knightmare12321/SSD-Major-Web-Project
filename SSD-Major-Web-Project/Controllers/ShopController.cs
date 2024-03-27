using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SSD_Major_Web_Project.Data.Services;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;
using System;
using System.Diagnostics.Metrics;
using System.Net;
using System.Numerics;

namespace SSD_Major_Web_Project.Controllers
{
    public class ShopController : Controller
    {

        private readonly ILogger<ShopController> _logger;
        private readonly NovaDbContext _context;
        private readonly IEmailService _emailService;

        public ShopController(ILogger<ShopController> logger, NovaDbContext context, IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            try
            {
                var shoppingCartCookie = Request.Cookies["cart"];
                List<ShoppingCartItem> ShoppingCartItemCookiesList = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(shoppingCartCookie);


                //Catch error, if shopping cart is empty(no products found), show a message"
                if (ShoppingCartItemCookiesList.Count() == 0)
                {
                    return View("Error", new ErrorViewModel { RequestId = "Your shopping cart is empty. Shop now"});
                }
                else
                {


                    List<ShoppingCartItem> shoppingcartItems = new List<ShoppingCartItem>();
                    //for (int i = 0; i < ShoppingCartItem.Count; i++)
                    //{
                    //    shoppingcartItems.Add(new ShoppingCartItem { SkuId = IDList[i], Quantity = 1 });
                    //}

                    shoppingcartItems = ShoppingCartItemCookiesList;

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


        [HttpPost]
        public IActionResult CheckCouponCode(string couponCode)
        {
            var discount = _context.Discounts.FirstOrDefault(d => d.PkDiscountCode == couponCode);
            bool isActive = false;
            string discountType = string.Empty;
            decimal discountAmount = 0;
            bool isValid = false;

            if (discount != null)
            {
                isActive = discount.IsActive;
                discountType = discount.DiscountType;
                DateOnly startDate = discount.StartDate;
                DateOnly endDate = discount.EndDate;

                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                if (currentDate >= startDate && currentDate <= endDate)
                {
                    isValid = true;
                    if (discountType == "Percent")
                    {
                        discountAmount = discount.DiscountValue / 100;
                    }
                    else if (discountType == "Number")
                    {
                        discountAmount = discount.DiscountValue;
                    }
                }
            }

            if (!isActive || !isValid)
            {
                discount = null;
            }

            return Json(new { discount, isActive, discountType, discountAmount, isValid});
        }


        // POST: ShopController/Checkout
        // Click on the "Proceed to checkout" button at shopping cart page, pass the shopping cart data to Checkout view
        [HttpPost]
        public IActionResult CheckoutShippingContact(ShoppingCartVM shoppingcartVM)
        {
            //Create a new Checkout view model object
            CheckoutVM checkoutVM = new CheckoutVM();

            checkoutVM.ShoppingCart = shoppingcartVM;

            // Assign Discount Code if available from the shopping cart razor view
            checkoutVM.ShoppingCart.CouponCode = shoppingcartVM.CouponCode != null ? shoppingcartVM.CouponCode : null;

            Discount discount = new Discount();
            // assignem the discount obect with the id
            discount = _context.Discounts.FirstOrDefault(d => d.PkDiscountCode == checkoutVM.ShoppingCart.CouponCode);


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
            List<ShoppingCartItem> ShoppingCartItemCookiesList = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(shoppingCartCookie);

            List<ShoppingCartItem> shoppingcartItems = new List<ShoppingCartItem>();
            shoppingcartItems = ShoppingCartItemCookiesList;
            //for (int i = 0; i < IDList.Count; i++)
            //{
            //    shoppingcartItems.Add(new ShoppingCartItem { SkuId = IDList[i], Quantity = 1 });
            //}

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

            checkoutVM.Order.Discount = discount;

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
            List<ShoppingCartItem> ShoppingCartItemCookiesList = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(shoppingCartCookie);

            List<ShoppingCartItem> shoppingcartItems = new List<ShoppingCartItem>();
            //for (int i = 0; i < ShoppingCartItem.Count; i++)
            //{
            //    shoppingcartItems.Add(new ShoppingCartItem { SkuId = IDList[i], Quantity = 1 });
            //}

            shoppingcartItems = ShoppingCartItemCookiesList;

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


                orderVM.OrderTotal = 0.0M;
                var subtotal = 0.0M;
                // Retrieve the unit price from the database based on the SKU ID
                var productSku = _context.ProductSkus.FirstOrDefault(p => p.PkSkuId == product.SkuId);
                if (productSku != null)
                {
                    // Assign the retrieved unit price to orderDetail.UnitPrice
                    singleOrderDetailRecord.UnitPrice = productSku.FkProduct?.Price ?? 0;
                     subtotal += singleOrderDetailRecord.UnitPrice * singleOrderDetailRecord.Quantity;           
              
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


                Tuple<string, int> result = _shopRepo.AddOrder(checkoutVM, contactId);

                string errorAddOrder = result.Item1;
                int orderId = result.Item2;
                checkoutVM.Order.OrderId = orderId;

                // add order details
                string errorAddOrderDetails = _shopRepo.AddOrderDetails(checkoutVM, orderId);

                // get coupon code from database by order id
                string discountCode = _context.Orders.FirstOrDefault(o => o.PkOrderId == orderId).FkDiscountCode;


            Discount discountCodeDb = _context.Discounts.FirstOrDefault(d => d.PkDiscountCode == discountCode);

            if (discountCodeDb != null)
            {
                checkoutVM.Order.Discount = discountCodeDb;
                checkoutVM.ShoppingCart.CouponCode = discountCodeDb.PkDiscountCode;
            }




                //string message;
                if (errorAddOrder == "")
                {
                    message = $"Order {checkoutVM.Order.OrderId} has been placed, you will get an email confirmation shortly once prcoeed payment.";
                    return View("Paypal", checkoutVM);
                }
                else
                {
                    message = $"Error placing new order. Please try again.";
                    ViewBag.Message = message;
                    return View("CheckoutShippingContact", checkoutVM);
                }


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

            // Compare and get the transaction ID from PayPal, update the order with the transaction ID,
            // make a request to PayPal using the transaction ID to get order details from PayPal,
            // and compare them with the order details we received in this method
            // compare amount and payername


            OrderConfirmationVM orderConfirmationVM = new OrderConfirmationVM();
            orderConfirmationVM.CheckoutVM = checkoutVM;

            if (!string.IsNullOrEmpty(orderConfirmationVM.CheckoutVM.TransactionId) && !orderConfirmationVM.CheckoutVM.TransactionId.Contains("not_valid"))
            {
                // At this point, the order is already created in the database
                // Select the order from the database using the order ID
                // Change the order status to "Paid" and update the order with the transaction ID

                Order order = _context.Orders.FirstOrDefault(o => o.PkOrderId == checkoutVM.Order.OrderId);
                if (order != null)
                {
                    // Change the order status to "Paid"
                    order.FkOrderStatusId = 2; // Assuming 2 represents the "Paid" status

                    // Update the order with the transaction ID
                    order.TransactionId = orderConfirmationVM.CheckoutVM.TransactionId;

        

                    _context.SaveChanges();
                    checkoutVM.Order.OrderStatus = "Paid";
                    // update the discount code to inactive, if only the discounttype is "Number"
                    Discount discount = _context.Discounts.FirstOrDefault(d => d.PkDiscountCode == order.FkDiscountCode);

                    // if any order with order status  paid and the discount type is "Number" and coupon code is equal to the discount code
                    // update the discount code to inactive
                    if (order.FkOrderStatusId == 2 && discount.DiscountType == "Number" && order.FkDiscountCode == discount.PkDiscountCode)
                    {
                        discount.IsActive = false;
                    }
                    _context.SaveChanges();


                }

                // use transactionId to find the order from the database
                // update checkoutVM
                Order orderBytransactionId = _context.Orders.FirstOrDefault(o => o.TransactionId == transactionId);
                ShoppingCartItem orderConfirmationCheckoutItemlist = new ShoppingCartItem();
                if (orderBytransactionId != null)
                {
                    ShoppingCartItem shoppingCartItem = new ShoppingCartItem();
                    // using the orderId to get all the cart item from db ( get the order id then use the order id to get the cart item)

                    List<OrderDetail> orderDetails = _context.OrderDetails.Where(o => o.FkOrderId == orderBytransactionId.PkOrderId).ToList();
                    List<ShoppingCartItem> shoppingCartItemList = new List<ShoppingCartItem>();
                    foreach (var orderDetail in orderDetails)
                    {
                        shoppingCartItem.SkuId = orderDetail.FkSkuId;
                        shoppingCartItem.Quantity = orderDetail.Quantity;
                        shoppingCartItemList.Add(shoppingCartItem);
                    }


                    checkoutVM.ShoppingCart.ShoppingCartItems = shoppingCartItemList;

                }

                //
    

                // Populate the orderconfirmationVM with the necessary data

                return View("OrderConfirmation",orderConfirmationVM);
                    }
                    else
                    {
                        // Return the Checkout page with the checkout view model again
                        return View("CheckoutShippingContact", checkoutVM);
                    }
                
              
            }

       

    }
}
