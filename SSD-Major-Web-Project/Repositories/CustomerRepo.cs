using Microsoft.AspNetCore.Mvc.Rendering;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Repositories
{
    public class CustomerRepo
    {
        private readonly NovaDbContext _db;

        public CustomerRepo(NovaDbContext context)
        {
            this._db = context;
        }

        public Customer RegisterUser(string email)
        {
            Customer customer = new Customer
            {
                PkCustomerId = email,
                FkUserTypeId = null,
                FkContactId = null,
            };

            _db.Customers.Add(customer);
            _db.SaveChanges();
            return customer;

        }

        /*        public string GetUsername(string email)
                {
                    Customer customer = _db.Customers.FirstOrDefault(x => x.PkCustomerId == email);
                    return $"{customer.FirstName}";
                }*/

        public IEnumerable<UserVM> GetAllUsers()
        {
            IEnumerable<UserVM> users =
            _db.Customers.Select(u => new UserVM { Email = u.PkCustomerId });

            return users;
        }

        public SelectList GetUserSelectList()
        {
            IEnumerable<SelectListItem> users =
                GetAllUsers().Select(u => new SelectListItem
                {
                    Value = u.Email,
                    Text = u.Email
                });

            SelectList roleSelectList = new SelectList(users,
                                                      "Value",
                                                      "Text");
            return roleSelectList;
        }
    }
}
