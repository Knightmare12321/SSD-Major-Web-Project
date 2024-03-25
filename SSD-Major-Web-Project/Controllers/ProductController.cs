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
            ViewBag.currentSku = 0;
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
                    FkCustomerId = r.FkCustomerId,
                    PkReviewDate = r.PkReviewDate,
                    Rating = r.Rating,
                    Comment = r.Comment
                }).ToList();
            ProductDetailVM? vm = products.GetByIdAndReviewVM(id, reviewList);
            return View(vm);
        }

        [HttpPost]
        public JsonResult UpdateCartButtonStatus(int skuid)
        {
            Console.WriteLine(skuid);

            return null;
        }

        [HttpPost]
        public JsonResult AddToCart(int id)
        {
            var cartCookie = Request.Cookies["cart"];
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(365);
            string resultJson;
            return Json(new { success = true, error = "" });
        }

        [HttpPost]
        public JsonResult AddToFavorite(int skuid)
        {
            var favoriteCookie = Request.Cookies["favorite"];
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(365);
            var favoriteSkuIDs = JsonConvert.DeserializeObject<List<int>>(favoriteCookie);
            if (favoriteSkuIDs.Contains(skuid))
            {
                return Json(new { success = false, error = "Item already in the wishlist!" });
            }
            favoriteSkuIDs.Add(skuid);
            Response.Cookies.Append("favorite", JsonConvert.SerializeObject(favoriteSkuIDs), option);
            return Json(new { success = true});
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
