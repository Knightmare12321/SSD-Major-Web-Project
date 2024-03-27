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

        // TODO: Filter non active item!
        public IActionResult Index(int? page)
        {
            int itemsPerPage = 12; // Number of items appear at the beginning
            var data = GetDataWithPages(page ?? 1, itemsPerPage);
            return View(data);
        }

        public IActionResult LoadMoreItems(int page)
        {
            var itemsPerPage = 12; // Number of items to load per page
            var results = GetDataWithPages(page, itemsPerPage);
            if (results.Any()) return PartialView("_ProductItemsPartial", results);
            return Content("");
        }

        private IEnumerable<ProductVM> GetDataWithPages(int page, int itemsPerPage)
        {
            ProductRepo temp = new ProductRepo(_context);
            var data = temp.GetAll();
            int maxSize = data.Count();
            List<ProductVM> results = new List<ProductVM>();
            for (int i = 0; i < itemsPerPage; i++)
            {
                int index = (page - 1) * itemsPerPage + i;
                if (index >= maxSize) break;
                results.Add(data.ElementAt(index));
            }
            return results;
        }

        // TODO: Remove unnecessary lines after ajax implementation completes
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


            List<ReviewVM> reviewList = reviews
                .Select(r => new ReviewVM
                {
                    FkCustomerId = r.FkCustomerId,
                    PkReviewDate = r.PkReviewDate,
                    Rating = r.Rating,
                    Comment = r.Comment
                }).ToList();
            ProductDetailVM? vm = products.GetByIdAndReviewVM(id, reviewList);
            return View(vm);
        }

        public IActionResult CreateReview(int id)
        {
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
