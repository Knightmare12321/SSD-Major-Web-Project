using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.Data.Services;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Controllers
{
    public class CustomerController : Controller
    {
        private readonly NovaDbContext _context;

        public CustomerController(NovaDbContext context)
        {
            _context = context;

        }
        // GET: CustomerController
        public ActionResult Index()
        {
            // get customerVM
            CustomerVM customerVM = new CustomerVM();
            // get current user by contactId
            var userId = User.Identity.Name;
            // Get the user details
            var customer = _context.Customers.Find(userId);
            customerVM.Customer = customer;
            customerVM.DefaultContact = _context.Contacts.Find(customer.FkContactId);

            return View(customerVM);
        }

        // GET: CustomerController/PersonalOrderHistory
        public ActionResult PersonalOrderHistory()
        {

            // Get customerVM
            CustomerVM customerVM = new CustomerVM();

            // Get current user by contactId
            var userId = User.Identity.Name;

            // Get the user details
            var customer = _context.Customers.Find(userId);
            customerVM.Customer = customer;

            // Get the user's orders
            var orders = _context.Orders.Where(o => o.FkCustomerId == userId).ToList();
            customerVM.Orders = orders;

            // Retrieve order details for each order
            Dictionary<int, List<OrderDetail>> orderDetailsDictionary = new Dictionary<int, List<OrderDetail>>();

            foreach (var order in orders)
            {
                var orderDetails = _context.OrderDetails.Where(od => od.FkOrderId == order.PkOrderId).ToList();
                orderDetailsDictionary.Add(order.PkOrderId, orderDetails);
            }

            customerVM.OrdersDetails = orderDetailsDictionary;


            // Retrieve product pictures for each order
            Dictionary<int, List<Image>> productPictures = new Dictionary<int, List<Image>>();

            foreach (var order in orders)
            {
                var orderDetails = orderDetailsDictionary[order.PkOrderId];
                var images = new List<Image>();

                foreach (var orderDetail in orderDetails)
                {
                    var skuId = orderDetail.FkSkuId;
                    var productSku = _context.ProductSkus.FirstOrDefault(s => s.PkSkuId == skuId);

                    if (productSku != null)
                    {
                        var productId = productSku.FkProductId;
                        var product = _context.Products.Find(productId);

                        if (product != null && product.ProductSkus != null && product.ProductSkus.Count > 0)
                        {
                            var productSkuWithImages = product.ProductSkus.FirstOrDefault(s => s.PkSkuId == skuId);
                            // only one image per product sku
                            var imagesForProduct = _context.Images.Where(i => i.FkProductId == productId).ToList();

                            images.AddRange(imagesForProduct);
                        }
                    }
                }

                productPictures.Add(order.PkOrderId, images);
            }

            customerVM.ProductPictures = productPictures;
            return View(customerVM);
        }

        // GET: CustomerController/OrderTracking
        // GET: CustomerController/OrderTracking
        public ActionResult OrderTracking()
        {
            return View();
        }

        // POST: CustomerController/OrderTracking
        [HttpPost]
        public ActionResult OrderTracking(int trackingNumber)
        {
            try
            {
                // get user id first
                var userId = User.Identity.Name;
                // validate if this is the user's order
                // if not, return error message
                // check if the order exists by tracking number, if yes, return the order details

                // new CustomerVM
                var customerVM = new CustomerVM();
                customerVM.TrackingNumber = trackingNumber;
                customerVM.Customer = _context.Customers.Find(userId);
                customerVM.DefaultContact = _context.Contacts.Find(customerVM.Customer.FkContactId);
                customerVM.Orders = _context.Orders.Where(o => o.FkCustomerId == userId).ToList();

                // find order by inquiring tracking number
                var order = _context.Orders.FirstOrDefault(o => o.Tracking == trackingNumber);



                if (order == null || order.FkCustomerId != userId)
                {
                    string message = "Tracking Number not valid";
                    ViewBag.Message = message;
                    return View("OrderTracking", customerVM);
                }
                else
                {
                    // get order id
                    var orderId = order.PkOrderId;

                    var orderbyTrackingId = _context.Orders.Where(o => o.PkOrderId == orderId).FirstOrDefault();

                    var trackiingResultVM = new TrackingResultVM();
                    trackiingResultVM.TrackingNumber = orderbyTrackingId.Tracking;

                    trackiingResultVM.OrderId = orderId.ToString();


                    var orderSatatusId = orderbyTrackingId.FkOrderStatusId;
                    var orderStatus = _context.OrderStatuses.Where(os => os.PkOrderStatusId == orderSatatusId).FirstOrDefault();
                    trackiingResultVM.OrderStatus = orderStatus.Status;



                    var shipDate = orderbyTrackingId.ShipDate;
                    trackiingResultVM.ShippedDate = shipDate
                        .HasValue ? shipDate.Value.ToString("dd/MM/yyyy") : "Not Shipped Yet";

                    //delivery date is null if the order is not delivered yet
                    // add 7 date of shipped date if the order is shipped
                    var deliveryDate = shipDate.HasValue ? shipDate.Value.AddDays(7) : (System.DateOnly?)null;

                    trackiingResultVM.DeliveryDate = deliveryDate.HasValue ? deliveryDate.Value.ToString("dd/MM/yyyy") : "Not Delivered Yet";


                    return View("TrackingResult", trackiingResultVM);
                }
            }
            catch
            {
                return View();
            }

        }

        // display order tracking result
        public ActionResult TrackingResult(Order orderMain)
        {
            return View(orderMain);
        }


        [Authorize]
        // GET: CustomerController/Profile
        public ActionResult Profile()
        {

            CustomerRepo customerRepo = new CustomerRepo(_context);
            // User must be logged in to view this page
            if (User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }


            // Get the current user
            var userId = User.Identity.Name;
            // Get the user details
            var customer = _context.Customers.Find(userId);

            // create new CustomerVM
            CustomerVM customerVM = new CustomerVM();
            customerVM.Customer = customer;

            // Get the default address, if it exists
            var defaultContact = customerRepo.GetUserContact(userId);

            // if this is null, create new address button

            if (defaultContact == null)
            {
                customerVM.DefaultContact = new Contact();
            }
            else
            {
                customerVM.DefaultContact = defaultContact;
            }

            return View(customerVM);
        }

        // GET: CustomerController/Profile
        public ActionResult EditProfile()
        {
            CustomerVM customerVM = new CustomerVM();
            // get current user by contactId
            var userId = User.Identity.Name;
            // Get the user details
            var customer = _context.Customers.Find(userId);
            customerVM.Customer = customer;
            customerVM.DefaultContact = _context.Contacts.Find(customer.FkContactId);

            return View(customerVM);

        }

        // POST: CustomerController/Profile
        [HttpPost]
        public ActionResult EditProfile(CustomerVM customerVM)
        {
            // use customer repo to update user contact
            CustomerRepo customerRepo = new CustomerRepo(_context);
            customerRepo.UpdateUserContact(User.Identity.Name, customerVM.DefaultContact);
            return View("Profile", customerVM);

        }


    }
}
