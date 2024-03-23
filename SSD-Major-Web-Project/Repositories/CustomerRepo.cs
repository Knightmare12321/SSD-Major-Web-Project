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
                FkContactId = null,
            };

            _db.Customers.Add(customer);
            _db.SaveChanges();
            return customer;

        }

        public string GetEmail(string email)
        {
            Customer customer = _db.Customers.FirstOrDefault(x => x.PkCustomerId == email);
            return $"{customer.PkCustomerId}";
        }

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

        public string GetContactId(string email)
        {
            Customer customer = _db.Customers.Find(email);
            return $"{customer.FkContactId}";
        }

        public string GetUserFIrstName(string email)
        {
            Customer customer = _db.Customers.Find(email);
            Contact contact = _db.Contacts.Find(customer.FkContactId);

            return $"{contact.FirstName}";
        }

        public string GetUserLastName(string email)
        {
            Customer customer = _db.Customers.Find(email);
            Contact contact = _db.Contacts.Find(customer.FkContactId);

            return $"{contact.LastName}";
        }

        public Contact GetUserContact(string email)
        {
            Customer customer = _db.Customers.Find(email);
            Contact contact = _db.Contacts.Find(customer.FkContactId);

            return contact;
        }

        // update user contact, create new if not exists
        public void UpdateUserContact(string email, Contact contact)
        {
            Customer customer = _db.Customers.Find(email);
            if (customer.FkContactId == null)
            {
                _db.Contacts.Add(contact);
                _db.SaveChanges();
                customer.FkContactId = contact.PkContactId;
                _db.SaveChanges();
            }
            else
            {
                Contact currentContact = _db.Contacts.Find(customer.FkContactId);
                currentContact.FirstName = contact.FirstName;
                currentContact.LastName = contact.LastName;
                currentContact.Address = contact.Address;
                currentContact.Address2 = contact.Address2;
                currentContact.City = contact.City;
                currentContact.Province = contact.Province;
                currentContact.Country = contact.Country;
                currentContact.PostalCode = contact.PostalCode;
                currentContact.PhoneNumber = contact.PhoneNumber;
                

                _db.SaveChanges();
            }
        }


        
    }
}
