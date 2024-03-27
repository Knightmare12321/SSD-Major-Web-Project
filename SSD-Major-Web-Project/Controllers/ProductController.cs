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

        //public IEnumerable<ProductVM> GetDataWithPages(int page, int itemsPerPage)
        //{

        //}

        // TODO: Remove unnecessary lines after ajax implementation completes
        public IActionResult Details(int id, string category, string method, int subID)
        {
            ProductRepo products = new ProductRepo(_context);
            ReviewRepo reviewRepo = new ReviewRepo(_context);
            List<Review> reviews = reviewRepo.GetReviewsForProduct(id);

            var favoriteCookie = Request.Cookies["favorite"];
            int skuID = subID == null ? products.GetSkuIdById(id) : subID;

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

        // TODO: Fix Json Reponse so that it will not return cart object(if needed)
        [HttpPost]
        public JsonResult AddToCart(int id, int quantity)
        {
            var cartCookie = Request.Cookies["cart"];
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(365);
            var carts = cartCookie == null ?
                new List<ShoppingCartItem>() :
                JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cartCookie);
            bool inShoppingCart = false;
            ShoppingCartItem mycart = null;
            string message = "";
            foreach (var item in carts)
            {
                if (item.SkuId == id)
                {
                    mycart = item;
                    inShoppingCart = true;
                    item.Quantity += quantity;
                    message = "Adding item quantity by " + quantity;
                    break;
                }
            }
            if (!inShoppingCart)
            {
                mycart = new ShoppingCartItem { SkuId = id, Quantity = quantity };
                carts.Add(mycart);
                message = "Adding item to shopping cart";
            }
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(carts), option);
            return Json(new { success = true, message = message, cart = mycart }); 
        }

        [HttpPost]
        public JsonResult AddToFavorite(int id)
        {
            var favoriteCookie = Request.Cookies["favorite"];
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(365);
            var favoriteIDs = favoriteCookie == null ?
                new List<int>() :
                JsonConvert.DeserializeObject<List<int>>(favoriteCookie);
            if (favoriteIDs.Contains(id))
            {
                return Json(new { success = false, message = "Item already in the wishlist!" });
            }
            favoriteIDs.Add(id);
            Response.Cookies.Append("favorite", JsonConvert.SerializeObject(favoriteIDs), option);
            return Json(new { success = true, message = "Item added to favorite successfully!"});
        }

        [HttpPost]
        public JsonResult RemoveFromFavorite(int id)
        {
            var favoriteCookie = Request.Cookies["favorite"];
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(365);
            if (favoriteCookie == null) return Json(new { success = false, message = "Wishlist is empty!" });
            var favoriteIDs = JsonConvert.DeserializeObject<List<int>>(favoriteCookie);
            if (!favoriteIDs.Contains(id)) return Json(new { success = false, message = "Item is not in the wishlist!" });
            favoriteIDs.Remove(id);
            Response.Cookies.Append("favorite", JsonConvert.SerializeObject(favoriteIDs), option);
            return Json(new { success = true, message = "Item removed from favorite successfully!" });
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
