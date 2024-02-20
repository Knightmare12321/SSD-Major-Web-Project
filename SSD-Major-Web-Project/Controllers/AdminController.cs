﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
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
            return View(vm);
        }


        //public IActionResult AdminOrderItems()
        //{
        //    AdminRepository adminRepository = new AdminRepository(_context);
        //    IQueryable<OrderItemVM> orders = adminRepository.GetAllOrderItems();

        //    //Separate orders based on order status
        //    List<OrderItemVM> pendingOrders = orders.Where(o => o.OrderStatus.Equals("Pending")).ToList();
        //    List<OrderItemVM> openOrders = orders.Where(o => o.OrderStatus.Equals("Paid")).ToList();
        //    List<OrderItemVM> shippedOrders = orders.Where(o => o.OrderStatus.Equals("Shipped")).ToList();
        //    List<OrderItemVM> deliveredOrders = orders.Where(o => o.OrderStatus.Equals("Delivered")).ToList();
        //    AdminOrderVM vm = new AdminOrderVM() { AllOrders = orders.ToList(), PendingOrders = pendingOrders, OpenOrders = openOrders, ShippedOrders = shippedOrders, DeliveredOrders = deliveredOrders };
        //    return View(vm);
        //}

        public IActionResult AdminOrder()
        {
            AdminRepository adminRepository = new AdminRepository(_context);
            IQueryable<OrderVM> orders = adminRepository.GetAllOrders();

            //Separate orders based on order status
            List<OrderVM> pendingOrders = orders.Where(o => o.OrderStatus.Equals("Pending")).ToList();
            List<OrderVM> openOrders = orders.Where(o => o.OrderStatus.Equals("Paid")).ToList();
            List<OrderVM> shippedOrders = orders.Where(o => o.OrderStatus.Equals("Shipped")).ToList();
            List<OrderVM> deliveredOrders = orders.Where(o => o.OrderStatus.Equals("Delivered")).ToList();
            AdminOrderVM vm = new AdminOrderVM() { AllOrders = orders.ToList(), PendingOrders = pendingOrders, OpenOrders = openOrders, ShippedOrders = shippedOrders, DeliveredOrders = deliveredOrders };
            return View(vm);
        }
    }
}
