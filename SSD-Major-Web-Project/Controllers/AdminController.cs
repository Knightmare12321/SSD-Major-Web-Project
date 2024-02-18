using Microsoft.AspNetCore.Mvc;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly NovaDbContext _context;
        public AdminController(NovaDbContext context)
        {
            _context = context;
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
                ProductRepository productRepository = new ProductRepository(_context);
                productRepository.AddProduct(vm.Name, vm.Price, vm.Description, vm.IsActive, vm.Image, vm.Sizes);
                return View("Index");
            }
            CreateProductVM newVM = new CreateProductVM();
            newVM = vm;
            return View(newVM);
        }


        public IActionResult AdminOrder()
        {
            CreateProductVM vm = new CreateProductVM();
            return View(vm);
        }
    }
}
