using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.Repositories
{
    public class CustomerRepo
    {
        private readonly NovaDbContext _db;

        public CustomerRepo(NovaDbContext context)
        {
            this._db = context;
        }

        public Customer RegisterUser(string lastName, string firstName, string email)
        {
            Customer customer = new Customer
            {
                LastName = lastName,
                FirstName = firstName,
                PkCustomerId = email
            };

            _db.Customers.Add(customer);
            _db.SaveChanges();
            return customer;

        }

        public string GetUsername(string email)
        {
            Customer customer = _db.Customers.FirstOrDefault(x => x.PkCustomerId == email);
            return $"{customer.FirstName}";
        }
    }
}
