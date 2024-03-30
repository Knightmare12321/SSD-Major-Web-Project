using Microsoft.AspNetCore.Mvc;
using System.Web;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;
using Newtonsoft.Json;
using EllipticCurve.Utils;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks.Dataflow;

namespace SSD_Major_Web_Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NovaDbContext _context;

        public ProductController(ILogger<HomeController> logger, NovaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // TODO: Search bar!
        //       Fix load efficiency.
        //       Fix load more button so it disappears as first opportunity.
        public IActionResult Index(int page = 1, string searchTerm = "")
        {
            ViewData["SearchTerm"] = searchTerm;
            ViewData["Page"] = page;
            int itemsPerPage = 12; // Number of items appear at the beginning
            var data = GetDataWithPages(page, itemsPerPage, searchTerm);
            return View(data);
        }

        public IActionResult LoadMoreItems(int page, string searchTerm = "")
        {
            int itemsPerPage = 12; // Number of items to load per page
            var results = GetDataWithPages(page, itemsPerPage, searchTerm);
            if (results.Any()) return PartialView("_ProductItemsPartial", results);
            return Content("");
        }

        private IEnumerable<ProductVM> GetDataWithPages(int page, int itemsPerPage, string searchTerm = "")
        {
            ProductRepo temp = new ProductRepo(_context);
            return temp.GetAllActiveWithPages(page, itemsPerPage, searchTerm);

        }

        public IActionResult Details(int id)
        {
            ProductRepo products = new ProductRepo(_context);
            ReviewRepo reviewRepo = new ReviewRepo(_context);
            List<Review> reviews = reviewRepo.GetReviewsForProduct(id);

            var favoriteCookie = Request.Cookies["favorite"];

            ViewBag.isFav =
                favoriteCookie == null ?
                false :
                JsonConvert.DeserializeObject<List<int>>(favoriteCookie).Contains(id);

            // Check if the currently signed-in user has bought the item
            string currentUserID = User.Identity.Name;
            // Check if the user has purchased the item
            bool hasPurchased = _context.Customers
                .Join(_context.Orders, customer => customer.PkCustomerId, order => order.FkCustomerId,
                    (customer, order) => new { customer, order })
                .Join(_context.OrderDetails, c => c.order.PkOrderId, orderDetail => orderDetail.FkOrderId,
                    (c, orderDetail) => new { c.customer, c.order, orderDetail })
                .Join(_context.ProductSkus, o => o.orderDetail.FkSkuId, productSku => productSku.PkSkuId,
                    (o, productSku) => new { o.customer, o.order, o.orderDetail, productSku })
                .Join(_context.Products, p => p.productSku.FkProductId, product => product.PkProductId,
                    (p, product) => new { p.customer, p.order, p.orderDetail, p.productSku, product })
                .Any(joined => joined.customer.PkCustomerId == currentUserID && joined.product.PkProductId == id);

            // Check if the user has reviewed the item
            bool hasReviewed = reviews.Any(r => r.FkCustomerId == currentUserID);


            List<ReviewVM> reviewList = reviews
                .Select(r => new ReviewVM
                {
                    FkCustomerId = r.FkCustomerId,
                    PkReviewDate = r.PkReviewDate,
                    Rating = r.Rating,
                    Comment = r.Comment
                }).ToList();
            ProductDetailVM? vm = products.GetByIdAndReviewVM(id, reviewList);

            ViewBag.hasPurchased = hasPurchased;
            ViewBag.hasReviewed = hasReviewed;
            return View(vm);
        }

        public IActionResult CreateReview(int id)
        {
/*            var result = (from customer in _context.Customers
                          join order in _context.Orders on customer.PkCustomerId equals order.FkCustomerId
                          join orderDetail in _context.OrderDetails on order.PkOrderId equals orderDetail.FkOrderId
                          join productSku in _context.ProductSkus on orderDetail.FkSkuId equals productSku.PkSkuId
                          join product in _context.Products on productSku.FkProductId equals product.PkProductId
                          select new
                          {
                              CustomerId = customer.PkCustomerId,
                              OrderId = order.PkOrderId,
                              ProductName = product.Name,
                              HasReview = _context.Reviews.Any(r => r.FkCustomerId == customer.PkCustomerId && r.FkProductId == product.PkProductId)
                          }).FirstOrDefault();

            if (result != null)
            {
                if (!result.HasReview)
                {
                    ReviewVM reviewVM = new ReviewVM { FkCustomerId = User.Identity.Name, PkReviewDate = DateOnly.FromDateTime(DateTime.Now), Rating = 5, FkProductId = id };

                    return View(reviewVM);
                }
                
            }
            return View();*/

            ReviewVM reviewVM = new ReviewVM { FkCustomerId = User.Identity.Name, PkReviewDate = DateOnly.FromDateTime(DateTime.Now), Rating = 5, FkProductId = id };

            return View(reviewVM);

        }

        [HttpPost]
        public IActionResult CreateReview(ReviewVM reviewVM)
        {
            ReviewRepo reviewRepo = new ReviewRepo(_context);

            reviewVM.FkCustomerId = User.Identity.Name;
            string addMessage = reviewRepo.Add(reviewVM);

            return RedirectToAction("Index", new { message = addMessage });

        }


        // This controller action is for testing only.
        // Delete this after project completes!
        public IActionResult Favorite()
        {
            var IDListCookie = Request.Cookies["favorite"];
            List<ProductSku> results = new List<ProductSku>();
            if (IDListCookie != null)
            {
                List<int> IDList = JsonConvert.DeserializeObject<List<int>>(IDListCookie);
                ProductRepo products = new ProductRepo(_context);
                foreach (var id in IDList)
                {
                    results.Add(products.GetSkuById(id));
                }
            }

            return View(results);

        }
    }
}
