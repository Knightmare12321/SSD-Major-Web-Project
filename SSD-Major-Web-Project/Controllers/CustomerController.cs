using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSD_Major_Web_Project.Data.Services;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Controllers
{
    public class CustomerController : Controller
    {
        private readonly NovaDbContext _context;

        public CustomerController(NovaDbContext context)
        {
            _context = context;

        }
        // GET: CustomerController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CustomerController/PersonalOrderHistory
        public ActionResult PersonalOrderHistory()
        {
            return View();
        }

        // GET: CustomerController/OrderTracking
        public ActionResult OrderTracking()
        {
            return View();
        }

        // POST: CustomerController/OrderTracking
        [HttpPost]
        public ActionResult OrderTracking(int trackingNumber)
        {
            try
            {
                Order order = _context.Orders.Find(trackingNumber);
                return View(order);
            }
            catch
            {
                return View();
            }

            
        }


        [Authorize]
        // GET: CustomerController/Profile
        public ActionResult Profile()
        {

            CustomerRepo customerRepo = new CustomerRepo(_context);
            // User must be logged in to view this page
            if (User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }


            // Get the current user
            var userId = User.Identity.Name;
            // Get the user details
            var customer = _context.Customers.Find(userId);

            // create new CustomerVM
            CustomerVM customerVM = new CustomerVM();
            customerVM.Customer = customer;

            // Get the default address, if it exists
            var defaultContact = customerRepo.GetUserContact(userId);

            // if this is null, create new address button

            if (defaultContact == null)
            {
                customerVM.DefaultContact = new Contact();
            }
            else
            {
                customerVM.DefaultContact = defaultContact;
            }

            return View(customerVM);
        }

        // GET: CustomerController/Profile
        public ActionResult EditProfile()
        {
            CustomerVM customerVM = new CustomerVM();
            // get current user by contactId
            var userId = User.Identity.Name;
            // Get the user details
            var customer = _context.Customers.Find(userId);
            customerVM.Customer = customer;
            customerVM.DefaultContact = _context.Contacts.Find(customer.FkContactId);

            return View(customerVM);
    
        }

        // POST: CustomerController/Profile
        [HttpPost]
        public ActionResult EditProfile(CustomerVM customerVM)
        {
            // use customer repo to update user contact
            CustomerRepo customerRepo = new CustomerRepo(_context);
            customerRepo.UpdateUserContact(User.Identity.Name, customerVM.DefaultContact);
            return View("Profile", customerVM);

        }


    }
}
