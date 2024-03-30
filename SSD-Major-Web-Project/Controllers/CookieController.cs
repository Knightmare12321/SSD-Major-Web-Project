using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.Controllers
{
    public class CookieController : Controller
    {
        private readonly ILogger<CookieController> _logger;
        private readonly NovaDbContext _context;

        public CookieController(ILogger<CookieController> logger, NovaDbContext context)
        {
            _logger = logger;
            _context = context;
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
                    message = "Adding item quantity by " + quantity
                        + " (current quantity: " + item.Quantity + ")";
                    break;
                }
            }
            if (!inShoppingCart)
            {
                mycart = new ShoppingCartItem { SkuId = id, Quantity = quantity };
                carts.Add(mycart);
                message = "Item added to shopping cart!";
            }
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(carts), option);
            return Json(new { success = true, message = message, cart = mycart });
        }

        [HttpPost]
        public JsonResult RemoveFromCart(int id, bool allQuantity = false)
        {
            var cartCookie = Request.Cookies["cart"];
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(365);
            var carts = cartCookie == null ?
                new List<ShoppingCartItem>() :
                JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cartCookie);
            bool inShoppingCart = false;
            foreach (var item in carts)
            {
                if (item.SkuId == id)
                {
                    if (item.Quantity == 1 || allQuantity == true)
                    {
                        carts.Remove(item);
                    }
                    else
                    {
                        item.Quantity -= 1;
                    }
                    inShoppingCart = true;
                    break;
                }
            }
            if (!inShoppingCart) return Json(new { success = false, message = "No such item in the cart!" });
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(carts), option);
            return Json(new { success = true, message = "Item removed from cart successfully!" });
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
            return Json(new { success = true, message = "Item added to favorite successfully!" });
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
    }
}
