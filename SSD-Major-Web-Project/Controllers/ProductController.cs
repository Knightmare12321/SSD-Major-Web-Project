using Microsoft.AspNetCore.Mvc;
using System.Web;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;
using Newtonsoft.Json;
using EllipticCurve.Utils;

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

        public IActionResult Details(int id, string addType)
        {
            ProductRepo products = new ProductRepo(_context);
            if (addType == "cart" || addType == "favorite")
            {
                // Set the expired date when cookie is updated/created.
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(365);

                var IDListCookie = Request.Cookies[addType];
                /* If the cookie doesn't exist, create a new cookie.
                   If the cookie exists, deserialize the ID list and and add new ID to the list.
                   Pass in cookie with serialized ID list. */
                if (IDListCookie == null)
                {
                    List<int> IDList = new List<int>();
                    IDList.Add(id);
                    string resultJson = JsonConvert.SerializeObject(IDList);
                    Response.Cookies.Append(addType, resultJson, option);
                }
                else
                {
                    List<int> IDList = JsonConvert.DeserializeObject<List<int>>(IDListCookie);
                    IDList.Add(id);
                    string resultJson = JsonConvert.SerializeObject(IDList);
                    Response.Cookies.Append(addType, resultJson, option);
                }

            }

            ProductDetailVM? vm = products.GetByIdVM(id);
            return View(vm);
        }

        public IActionResult Favorite()
        {
            var IDListCookie = Request.Cookies["cart"];
            List<Product> results = new List<Product>();
            if (IDListCookie != null)
            {
                List<int> IDList = JsonConvert.DeserializeObject<List<int>>(IDListCookie);
                ProductRepo products = new ProductRepo(_context);
                foreach (var id in IDList)
                {
                    results.Add(products.GetById(id));
                }
            }

            return View(results);
            
        }
    }
}
