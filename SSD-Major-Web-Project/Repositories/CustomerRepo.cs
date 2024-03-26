using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Internal;
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

        public IQueryable<PersonalOrderHistoryVM> GetOrders(string customerId)
        {
            //get all orders and its navigational properties needed
            var query = _db.Orders.Where(o => o.FkCustomerId == customerId).Select(o => new PersonalOrderHistoryVM
            {
                OrderId = o.PkOrderId,
                OrderDate = o.OrderDate,
                TransactionId = o.TransactionId,
                ShipDate = o.ShipDate,
                Tracking = o.Tracking,
                OrderStatus = _db.OrderStatuses
                                .Where(os => os.PkOrderStatusId == o.FkOrderStatusId)
                                .Select(os => os.Status)
                                .FirstOrDefault()
                                .ToString(),
                BuyerNote = o.BuyerNote,
                OrderDetails = _db.OrderDetails
                                .Where(od => od.FkOrderId == o.PkOrderId)
                                .Select(od => new OrderDetail
                                {
                                    Quantity = od.Quantity,
                                    UnitPrice = od.UnitPrice,
                                    FkSku = _db.ProductSkus
                                        .Where(psku => psku.PkSkuId == od.FkSkuId)
                                        .Select(fsku => new ProductSku
                                        {
                                            Size = fsku.Size,
                                            FkProduct = _db.Products
                                                .Where(p => p.PkProductId == fsku.FkProductId)
                                                .Select(p => new Product
                                                {
                                                    PkProductId = p.PkProductId,
                                                    Name = p.Name,
                                                    Price = p.Price,
                                                    Description = p.Description,
                                                    IsActive = p.IsActive,
                                                    Images = _db.Images
                                                                .Where(i => i.FkProductId == p.PkProductId).ToList()
                                                })
                                                .FirstOrDefault()
                                        }).FirstOrDefault()
                                }).ToList(),
                Contact = _db.Contacts
                        .Where(u => u.PkContactId == o.FkContactId)
                        .FirstOrDefault(),
                Discount = _db.Discounts
                        .Where(d => d.PkDiscountCode == o.FkDiscountCode)
                        .FirstOrDefault(),

                OrderTotal = Math.Round(_db.Orders
                    .Join(_db.OrderDetails,
                            o => o.PkOrderId,
                            od => od.FkOrderId,
                            (o, od) => new
                            {
                                Order = o,
                                OrderDetail = od
                            })
                    .LeftJoin(_db.Discounts,
                            ood => ood.Order.FkDiscountCode,
                            d => d.PkDiscountCode,
                            (ood, d) => new
                            {
                                ood.Order,
                                ood.OrderDetail,
                                Discount = d
                            })
                    .Where(order => order.OrderDetail.FkOrderId == o.PkOrderId)
                    .Select((order) => order.OrderDetail.Quantity * order.OrderDetail.UnitPrice)
                    .Sum(), 2)
            });

            return query;
        }



    }
}
