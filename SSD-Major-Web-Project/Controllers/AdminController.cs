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
using System.Drawing.Printing;
using System.Collections;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.AspNetCore.Authorization;


namespace SSD_Major_Web_Project.Controllers
{
    [Authorize(Roles = "Admin, Manager")]

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

        public IActionResult AllProducts(string message = "", string searchTerm = "", bool showInactive = false, int pageIndex = 1, int pageSize = 10)
        {
            ViewData["Message"] = message;
            ViewData["SearchTerm"] = searchTerm;
            ViewData["ShowInactive"] = showInactive;
            AdminRepo adminRepo = new AdminRepo(_context);
            List<AdminProductVM> products = adminRepo.GetAllProducts(searchTerm, showInactive).ToList();
            //determine which items to show based on current page index
            var count = products.Count();
            var items = products.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var paginatedProducts = new PaginatedList<AdminProductVM>(items
                                                            , count
                                                            , pageIndex
                                                            , pageSize);
            return View(paginatedProducts);
        }

        public IActionResult CreateProduct()
        {
            ViewData["Create"] = true;
            CreateProductVM vm = new CreateProductVM();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductVM inputVM)
        {
            UtilityRepo utilityRepo = new UtilityRepo();
            CreateProductVM vm = utilityRepo.FilterHarmfulInput(inputVM);

            if (ModelState.IsValid)
            {

                //make sure all images are in appropriate file types
                string contentType;
                foreach (IFormFile image in vm.Images)
                {
                    contentType = image.ContentType;
                    if (contentType != "image/png" &&
                    contentType != "image/jpeg" &&
                    contentType != "image/jpg")
                    {
                        ModelState.AddModelError("imageUpload", "Please upload a PNG, " +
                                        "JPG, or JPEG file.");
                        return View(vm);

                    }
                }

                AdminRepo adminRepo = new AdminRepo(_context);
                string errorString = await adminRepo.AddProduct(vm.Name, vm.Price, vm.Description, vm.IsActive, vm.Images, vm.Sizes);

                string message;
                if (errorString == "")
                {
                    message = "The product was created successfully";
                }
                else
                {
                    message = errorString;
                }
                return RedirectToAction("AllProducts", new { message = message });

            }
            ViewData["Create"] = true;
            return View(inputVM);
        }

        public IActionResult EditProduct(int productId)
        {
            ViewData["Create"] = false;
            AdminRepo adminRepo = new AdminRepo(_context);
            Product product = adminRepo.GetProductById(productId);
            CreateProductVM vm = new CreateProductVM
            {
                PkProductId = product.PkProductId,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                IsActive = product.IsActive,
            };

            //add sizes to vm
            List<string> sizes = new List<string>();
            foreach (var item in product.ProductSkus)
            {
                sizes.Add(item.Size);
            }
            vm.Sizes = sizes;

            //add image files to vm
            List<IFormFile> files = new List<IFormFile>();
            foreach (var item in product.Images)
            {
                var stream = new MemoryStream(item.Data);
                IFormFile file = new FormFile(stream, 0, item.Data.Length, item.AltText, item.FileName);
                files.Add(file);
            }
            vm.Images = files;

            return View("CreateProduct", vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(CreateProductVM vm)
        {
            ViewData["Create"] = false;

            AdminRepo adminRepo = new AdminRepo(_context);

            //check if user has uploaded new image or decide to keep any previous one from the database
            bool hasImage = adminRepo.checkProductHasImage(vm.PkProductId, vm.Images, vm.DeletedImageNames);

            //if no image is left to associate with the product, return the view with error 
            if (!hasImage)
            {
                ModelState.AddModelError("imageUpload", "The listing needs at least one image added");
                Product product = adminRepo.GetProductById(vm.PkProductId);
                //add sizes to vm
                List<string> sizes = new List<string>();
                foreach (var item in product.ProductSkus)
                {
                    sizes.Add(item.Size);
                }
                vm.Sizes = sizes;

                //add image files to vm
                List<IFormFile> files = new List<IFormFile>();
                foreach (var item in product.Images)
                {
                    var stream = new MemoryStream(item.Data);
                    IFormFile file = new FormFile(stream, 0, item.Data.Length, item.AltText, item.FileName);
                    files.Add(file);
                }
                vm.Images = files;

                return View("CreateProduct", vm);
            }

            //check if view model content is valid
            if (ModelState.IsValid)
            {
                //make sure all images are in appropriate file types
                string contentType;
                foreach (IFormFile image in vm.Images)
                {
                    contentType = image.ContentType;
                    if (contentType != "image/png" &&
                    contentType != "image/jpeg" &&
                    contentType != "image/jpg")
                    {
                        ModelState.AddModelError("imageUpload", "Please upload a PNG, " +
                                        "JPG, or JPEG file.");
                        return View("CreateProduct", vm);

                    }
                }

                string errorString = await adminRepo.EditProduct(vm.PkProductId, vm.Name, vm.Price, vm.Description, vm.IsActive, vm.Images, vm.Sizes, vm.DeletedImageNames);

                string message;
                if (errorString == "")
                {
                    message = "The product was successfully editted";
                }
                else
                {
                    message = errorString;
                }
                return RedirectToAction("AllProducts", new { message = message });

            }

            ViewData["Create"] = false;
            return View("CreateProduct", vm);
        }


        public IActionResult DeactivateProduct(int productId)
        {
            AdminRepo adminRepo = new AdminRepo(_context);
            Product product = adminRepo.GetProductById(productId);
            AdminProductVM vm = new AdminProductVM
            {
                PkProductId = product.PkProductId,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                IsActive = product.IsActive,
                Images = product.Images,
                ProductSkus = product.ProductSkus
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult DeactivateProduct(AdminProductVM vm)
        {
            AdminRepo adminRepo = new AdminRepo(_context);
            string message = adminRepo.DeactivateProduct(vm.PkProductId);
            return RedirectToAction("AllProducts", new { message = message });
        }

        public IActionResult AdminOrder(int pageIndex = 1, int pageSize = 5)
        {
            AdminRepo adminRepo = new AdminRepo(_context);
            ViewData["OrderStatus"] = "Paid";

            //show the open orders as default on the page
            List<OrderVM> orders = adminRepo.GetFilteredOrders("Paid").ToList();
            var count = orders.Count();
            var items = orders.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var paginatedOrders = new PaginatedList<OrderVM>(items
                                                           , count
                                                           , pageIndex
                                                           , pageSize);
            return View(paginatedOrders);
        }

        public IActionResult GetFilteredOrders(string orderStatus = "", string searchTerm = "", int pageIndex = 1, int pageSize = 5)
        {
            ViewData["OrderStatus"] = orderStatus;
            AdminRepo adminRepo = new AdminRepo(_context);
            List<OrderVM> orders = adminRepo.GetFilteredOrders(orderStatus, searchTerm).ToList();

            //determine which items to show based on current page index
            var count = orders.Count();
            var items = orders.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var paginatedOrders = new PaginatedList<OrderVM>(items
                                                            , count
                                                            , pageIndex
                                                            , pageSize);

            return PartialView("_OrderSummaryPartial", paginatedOrders);

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
        public async Task<JsonResult> CancelOrder(int orderId)
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

                //send refund as a discount code in email
                var response = await _emailService.SendSingleEmail(new Models.ComposeEmailModel
                {
                    FirstName = "Nova",
                    LastName = "Clothing",
                    Subject = $"Nova Fashion Order (#{orderId}) Cancelled",
                    Email = order.CustomerId,
                    Body = $"Your order (#{order.OrderId}) of {order.OrderTotal:C} has been " +
                    $"refunded. The credit has been added to the discount code {discountCode} and will " +
                    $"expire on {endDate}."
                });

                return Json(new { success = true, error = "" });
            }
            else if (order.OrderStatus.ToLower() == "cancelled")
            {
                return Json(new
                {
                    success = false,
                    error = "Order was already cancelled so no action was taken"
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


        public IActionResult AllDiscounts(string message = "", string searchTerm = "", bool showInactive = false, int pageIndex = 1, int pageSize = 10)
        {
            ViewData["Message"] = message;
            ViewData["SearchTerm"] = searchTerm;
            ViewData["ShowInactive"] = showInactive;
            AdminRepo adminRepo = new AdminRepo(_context);
            List<DiscountVM> discounts = adminRepo.GetAllDiscounts(searchTerm, showInactive).ToList();

            //determine which items to show based on current page index
            var count = discounts.Count();
            var items = discounts.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var paginatedDiscounts = new PaginatedList<DiscountVM>(items
                                                            , count
                                                            , pageIndex
                                                            , pageSize);
            return View(paginatedDiscounts);
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
                    return RedirectToAction("AllDiscounts", new { message = $"Discount {vm.PkDiscountCode} successfully created" });
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
            DiscountVM vm = adminRepo.GetDiscountById(discountCode);
            return View(vm);
        }

        [HttpPost]
        public IActionResult DeactivateDiscount(DiscountVM vm)
        {
            AdminRepo adminRepo = new AdminRepo(_context);
            string message = adminRepo.DeactivateDiscount(vm.PkDiscountCode);
            return RedirectToAction("AllDiscounts", new { message = message });
        }

    }
}
