using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SSD_Major_Web_Project.Data.Services;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace SSD_Major_Web_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly NovaDbContext _context;
        private readonly IEmailService _emailService;

        public AdminController(NovaDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateProduct()
        {
            CreateProductVM vm = new CreateProductVM();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductVM vm)
        {
            if (ModelState.IsValid)
            {
                string contentType = vm.Image.ContentType;

                if (contentType == "image/png" ||
                    contentType == "image/jpeg" ||
                    contentType == "image/jpg")
                {

                    AdminRepo adminRepo = new AdminRepo(_context);
                    await adminRepo.AddProduct(vm.Name, vm.Price, vm.Description, vm.IsActive, vm.Image, vm.Sizes);
                    return View("Index");
                }
                else
                {
                    ModelState.AddModelError("imageUpload", "Please upload a PNG, " +
                                                            "JPG, or JPEG file.");
                }

            }
            return View(vm);
        }

        public IActionResult AdminOrder()
        {
            AdminRepo adminRepo = new AdminRepo(_context);
            ViewData["OrderStatus"] = "Paid";

            //show the open orders as default on the page
            IQueryable<OrderVM> orders = adminRepo.GetOrdersByStatus("Paid");
            List<OrderVM> vm = orders.ToList();
            return View(vm);
        }

        public IActionResult GetOrdersByStatus(string orderStatus = "")
        {
            ViewData["OrderStatus"] = orderStatus;
            AdminRepo adminRepo = new AdminRepo(_context);
            IQueryable<OrderVM> orders = adminRepo.GetOrdersByStatus(orderStatus);
            List<OrderVM> vm = orders.ToList();
            return PartialView("_OrderSummaryPartial", vm);
        }

        [HttpPost]
        public JsonResult DispatchOrder(int orderId)
        {
            AdminRepo adminRepo = new AdminRepo(_context);
            string jsonString = adminRepo.dispatchOrder(orderId);

            return Json(jsonString);

        }

        [HttpPost]
        public async Task<string> RefundOrder(int orderId)
        {   //find order
            AdminRepo adminRepo = new AdminRepo(_context);
            OrderVM order = adminRepo.GetOrderById(orderId);

            if (new List<string> { "paid", "shipped", "delivered" }.Contains(order.OrderStatus.ToLower()))
            {
                //create a discount with refund amount
                string discountCode = GetRandomString(15);
                decimal discountValue = order.OrderTotal;
                string discountType = "Number";
                DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
                DateOnly endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(365));
                Discount discount = adminRepo.CreateDiscount(discountCode, discountValue, discountType, startDate, endDate);

                //cancel order
                string error = adminRepo.CancelOrder(orderId);
                if (error != "")
                {
                    return error;
                }

                //send refund as a discount code in email
                //var response = await _emailService.SendSingleEmail(new Models.ComposeEmailModel
                //{
                //    FirstName = "Nova",
                //    LastName = "Clothing",
                //    Subject = $"Nova Fashion Order (#{orderId}) Cancelled",
                //    Email = "afang324@gmail.com",
                //    Body = $"Your order (#{order.OrderId}) of ${order.OrderTotal} has been" +
                //    $"refunded. The credit has been added to the discount code {discountCode} and will" +
                //    $"expire on {endDate}."
                //});

                return "Order was successfully refunded";
            }
            else if (order.OrderStatus.ToLower() == "refunded")
            {
                return "Order was already refunded so no action was taken";
            }
            else
            {
                return "Order hasn't been paid. A refund is not possible";
            }


        }

        public static string GetRandomString(int size)
        {
            char[] chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

            byte[] data = new byte[4 * size];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }
    }
}
