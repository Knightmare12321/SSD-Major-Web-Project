using Microsoft.AspNetCore.Mvc;
using SSD_Major_Web_Project.Models;
using System.Diagnostics;

namespace SSD_Major_Web_Project.Controllers
{
	public class HomeController : Controller
	{
		private readonly ClothesDbContext _context;

		public HomeController(ClothesDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View(_context.Products);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}