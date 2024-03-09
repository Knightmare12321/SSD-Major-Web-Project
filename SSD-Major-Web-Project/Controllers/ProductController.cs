using Microsoft.AspNetCore.Mvc;
using System.Web;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;
using Newtonsoft.Json;
using EllipticCurve.Utils;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Hosting;

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

        public IActionResult Index()
        {
            ProductRepo temp = new ProductRepo(_context);
            return View(temp.GetAll());
        }

        public IActionResult Details(int id, string category, string method, int subID)
        {
            ProductRepo products = new ProductRepo(_context);
            ReviewRepo reviewRepo = new ReviewRepo(_context);
            List<Review> reviews = reviewRepo.GetReviewsForProduct(id);

            var cartCookie = Request.Cookies["cart"];
            var favoriteCookie = Request.Cookies["favorite"];
            int skuID = subID == null ? products.GetSkuIdById(id) : subID;

            ViewBag.productSkuID = 0;
            if (category == "cart" || category == "favorite")
            {
                // Set the expired date when cookie is updated/created.
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(365);
                string resultJson;

                var IDListCookie = category == "cart" ? cartCookie : favoriteCookie;
                /* If the cookie doesn't exist, create a new cookie.
                   If the cookie exists, deserialize the ID list and and add/delete ID in the list.
                   Pass in cookie with serialized ID list. */
                if (IDListCookie == null)
                {
                    List<int> IDList = new List<int>();
                    IDList.Add(skuID);
                    resultJson = JsonConvert.SerializeObject(IDList);
                    Response.Cookies.Append(category, resultJson, option);
                }
                else
                {
                    List<int> IDList = JsonConvert.DeserializeObject<List<int>>(IDListCookie);
                    if (method == "add") IDList.Add(skuID);
                    else if (method == "remove") IDList.Remove(skuID);
                    resultJson = JsonConvert.SerializeObject(IDList);
                    Response.Cookies.Append(category, resultJson, option);
                }
                if (category == "cart") cartCookie = resultJson;
                else favoriteCookie = resultJson;
            }

            ViewBag.isCart =
                cartCookie == null ?
                false :
                JsonConvert.DeserializeObject<List<int>>(cartCookie).Contains(skuID);
            ViewBag.isFav =
                favoriteCookie == null ?
                false :
                JsonConvert.DeserializeObject<List<int>>(favoriteCookie).Contains(skuID);


            List<ReviewVM> reviewList = reviews
                .Select(r => new ReviewVM
                {
                    PkReviewDate = r.PkReviewDate,
                    Rating = r.Rating,
                    Comment = r.Comment
                }).ToList();
            ProductDetailVM? vm = products.GetByIdAndReviewVM(id, reviewList);
            return View(vm);
        }

        public IActionResult CreateReview(int id)
        {
            Review review = new Review { PkReviewDate = DateOnly.FromDateTime(DateTime.Now), Rating = 1, FkProductId = id };

            return View(review);
        }

        [HttpPost]
        public IActionResult CreateReview(Review review)
        {
            ReviewRepo reviewRepo = new ReviewRepo(_context);

            string addMessage = reviewRepo.Add(review);

            return RedirectToAction("Index", new { message = addMessage });

        }


        // This controller action is for testing only.
        // Delete this after project completes!
        public IActionResult Favorite()
        {
            var IDListCookie = Request.Cookies["cart"];
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
