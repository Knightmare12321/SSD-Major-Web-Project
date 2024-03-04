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
using Microsoft.AspNetCore.Mvc.Rendering;


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
            IQueryable<OrderVM> orders = adminRepo.GetFilteredOrders("Paid");
            List<OrderVM> vm = orders.ToList();
            return View(vm);
        }

        public IActionResult GetFilteredOrders(string orderStatus = "", string searchTerm = "")
        {
            ViewData["OrderStatus"] = orderStatus;
            AdminRepo adminRepo = new AdminRepo(_context);
            IQueryable<OrderVM> orders = adminRepo.GetFilteredOrders(orderStatus, searchTerm);
            List<OrderVM> vm = orders.ToList();
            return PartialView("_OrderSummaryPartial", vm);
        }

        [HttpPost]
        public JsonResult DispatchOrder(int orderId)
        {
            AdminRepo adminRepo = new AdminRepo(_context);
            string errorString = adminRepo.dispatchOrder(orderId);

            if (errorString != "")
            {
                return Json(
                   new
                   {
                       success = false,
                       error = errorString
                   });
            }

            return Json(new
            {
                success = true,
                error = ""
            });

        }

        [HttpPost]
        public async Task<JsonResult> RefundOrder(int orderId)
        {   //find order
            AdminRepo adminRepo = new AdminRepo(_context);
            OrderVM order = adminRepo.GetOrderById(orderId);

            if (new List<string> { "paid", "shipped", "delivered" }.Contains(order.OrderStatus.ToLower()))
            {
                //create a discount with refund amount
                string discountCode = adminRepo.GetRandomString(15, "alphanumerical");
                decimal discountValue = order.OrderTotal;
                string discountType = "Number";
                DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
                DateOnly endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(365));
                string errorString = adminRepo.CreateDiscount(discountCode, discountValue, discountType, startDate, endDate);
                if (errorString != "")
                {
                    return Json(new
                    {
                        success = false,
                        error = "An unexpected error occured while creating coupon code for refund credit"
                    });
                }


                //cancel order
                errorString = adminRepo.CancelOrder(orderId);
                if (errorString != "")
                {
                    return Json(new
                    {
                        success = false,
                        error = errorString
                    });
                }

                string abc = order.Contact.Customers.ToList()[0].PkCustomerId;
                //send refund as a discount code in email
                var response = await _emailService.SendSingleEmail(new Models.ComposeEmailModel
                {
                    FirstName = "Nova",
                    LastName = "Clothing",
                    Subject = $"Nova Fashion Order (#{orderId}) Cancelled",
                    Email = order.Contact.Customers.ToList()[0].PkCustomerId,
                    Body = $"Your order (#{order.OrderId}) of {order.OrderTotal:C} has been " +
                    $"refunded. The credit has been added to the discount code {discountCode} and will " +
                    $"expire on {endDate}."
                });

                return Json(new { success = true, error = "" });
            }
            else if (order.OrderStatus.ToLower() == "refunded")
            {
                return Json(new
                {
                    success = false,
                    error = "Order was already refunded so no action was taken"
                });
            }
            else
            {
                return Json(new
                {
                    sucess = false,
                    error = "Order hasn't been paid. A refund is not possible"
                });
            }
        }


        public IActionResult GetAllDiscounts(string message = "")
        {
            ViewData["Message"] = message;
            AdminRepo adminRepo = new AdminRepo(_context);
            List<DiscountVM> discounts = adminRepo.GetAllDiscounts().ToList();
            return View(discounts);
        }

        public IActionResult CreateDiscount()
        {
            SelectList discountTypes = new SelectList(new List<string> { "Percent", "Number" });

            DiscountVM vm = new DiscountVM { DiscountTypes = discountTypes };
            return View(vm);
        }

        [HttpPost]
        public IActionResult CreateDiscount(DiscountVM vm)
        {
            if (ModelState.IsValid)
            {
                AdminRepo adminRepo = new AdminRepo(_context);

                string error = adminRepo.CreateDiscount(vm.PkDiscountCode,
                                                        vm.DiscountValue,
                                                        vm.DiscountType,
                                                        vm.StartDate,
                                                        vm.EndDate);
                if (error == "")
                {
                    return RedirectToAction("GetAllDiscounts", new { message = $"Discount {vm.PkDiscountCode} successfully created" });
                }
                else
                {
                    ModelState
                    .AddModelError("", error);
                }
            }
            SelectList discountTypes = new SelectList(new List<string> { "Percent", "Number" });
            vm.DiscountTypes = discountTypes;
            return View(vm);

        }

        public IActionResult DeactivateDiscount(string discountCode)
        {
            AdminRepo adminRepo = new AdminRepo(_context);
            Discount discount = adminRepo.GetDiscountById(discountCode);
            DiscountVM vm = new DiscountVM
            {
                PkDiscountCode = discountCode,
                DiscountValue = discount.DiscountValue,
                DiscountType = discount.DiscountType,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                IsActive = discount.IsActive
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult DeactivateDiscount(DiscountVM vm)
        {
            AdminRepo adminRepo = new AdminRepo(_context);
            string message = adminRepo.DeactivateDiscount(vm.PkDiscountCode);
            return RedirectToAction("GetAllDiscounts", new { message = message });
        }

    }
}
