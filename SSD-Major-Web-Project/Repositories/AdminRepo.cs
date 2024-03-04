using NuGet.Packaging.Signing;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace SSD_Major_Web_Project.Repositories
{
    public class AdminRepo
    {
        private readonly NovaDbContext _context;

        public AdminRepo(NovaDbContext context)
        {
            _context = context;
        }

        public async Task AddProduct(string name, decimal price, string description, string isActive, IFormFile imageFile, List<string> sizes)
        {

            try
            {
                //add image to db
                List<ProductSku> productSkus = new List<ProductSku>();
                for (int i = 0; i < sizes.Count; i++)
                {
                    productSkus.Add(new ProductSku { Size = sizes[i] });

                }
                Product product = new Product { Name = name, Price = price, Description = description, IsActive = isActive, ProductSkus = productSkus };
                _context.Products.Add(product);
                _context.SaveChanges();


                //convert image file data to byte[]
                byte[] imageData;
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }

                //add image to db
                Image image = new Image { FileName = imageFile.FileName, Data = imageData, AltText = "product photo ", FkProductId = product.PkProductId };
                _context.Images.Add(image);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public List<OrderVM> GetFilteredOrders(string orderStatus = "", string searchTerm = "")
        {
            //find status id of the given order status 
            int orderStatusId = _context.OrderStatuses
                .Where(os => os.Status == orderStatus)
                .Select(os => os.PkOrderStatusId)
                .FirstOrDefault();

            //get all orders and its navigational properties needed
            var query = _context.Orders.Where(o => orderStatus == "" || o.FkOrderStatusId == orderStatusId).Select(o => new OrderVM
            {
                OrderId = o.PkOrderId,
                OrderDate = o.OrderDate,
                OrderStatus = _context.OrderStatuses
                                .Where(os => os.PkOrderStatusId == o.FkOrderStatusId)
                                .Select(os => os.Status)
                                .FirstOrDefault()
                                .ToString(),
                BuyerNote = o.BuyerNote,
                OrderDetails = _context.OrderDetails
                                .Where(od => od.FkOrderId == o.PkOrderId)
                                .Select(od => new OrderDetail
                                {
                                    Quantity = od.Quantity,
                                    UnitPrice = od.UnitPrice,
                                    FkSku = _context.ProductSkus
                                        .Where(psku => psku.PkSkuId == od.FkSkuId)
                                        .Select(fsku => new ProductSku
                                        {
                                            Size = fsku.Size,
                                            FkProduct = _context.Products
                                                .Where(p => p.PkProductId == fsku.FkProductId)
                                                .FirstOrDefault()
                                        }).FirstOrDefault()
                                }).ToList(),
                Contact = _context.Contacts
                        .Where(u => u.PkContactId == o.FkContactId)
                        .FirstOrDefault(),
                Discount = _context.Discounts
                        .Where(d => d.PkDiscountCode == o.FkDiscountCode)
                        .FirstOrDefault(),

                OrderTotal = Math.Round(_context.Orders
                    .Join(_context.OrderDetails,
                            o => o.PkOrderId,
                            od => od.FkOrderId,
                            (o, od) => new
                            {
                                Order = o,
                                OrderDetail = od
                            })
                    .LeftJoin(_context.Discounts,
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

            //filter based on search term
            query.Where(o => o.OrderId.ToString().Contains(searchTerm) ||
                        o.OrderDate.ToString().Contains(searchTerm) ||
                        o.BuyerNote.Contains(searchTerm) ||
                        o.OrderDetails.Any(od => od.Quantity.ToString().Contains(searchTerm) ||
                                                 od.UnitPrice.ToString().Contains(searchTerm) ||
                                                 od.FkSku.FkProduct.Name.ToString().Contains(searchTerm)
                                        ) ||
                        o.Contact.FirstName.Contains(searchTerm) ||
                        o.Contact.LastName.Contains(searchTerm) ||
                        o.Contact.Address.Contains(searchTerm) ||
                        o.Contact.Address2.Contains(searchTerm) ||
                        o.Contact.City.Contains(searchTerm) ||
                        o.Contact.LastName.Contains(searchTerm) ||
                        o.Contact.Province.Contains(searchTerm) ||
                        o.Contact.Country.Contains(searchTerm) ||
                        o.Contact.PostalCode.Contains(searchTerm) ||
                        o.Contact.PhoneNumber.Contains(searchTerm) ||
                        o.Discount.PkDiscountCode.ToString().Contains(searchTerm) ||
                        o.Discount.DiscountValue.ToString().Contains(searchTerm) ||
                        o.OrderTotal.ToString().Contains(searchTerm)
                        );

            //apply discount to orderTotal of each order
            var newQuery = query.ToList();
            newQuery.ForEach(order =>
            {
                if (order.Discount != null)
                {
                    if (order.Discount.DiscountType.ToLower() == "percent")
                    {
                        order.OrderTotal = order.OrderTotal * (1 - order.Discount.DiscountValue / 100);
                    }
                    else
                    {
                        order.OrderTotal = order.OrderTotal - order.Discount.DiscountValue;
                    }
                }
            });



            return newQuery;
        }

        public string dispatchOrder(int orderId)
        {
            try
            {
                Order order = _context.Orders.Where(o => o.PkOrderId == orderId).FirstOrDefault();
                string status = _context.OrderStatuses.Where(os => os.PkOrderStatusId == order.FkOrderStatusId).Select(os => os.Status).FirstOrDefault();

                //check if order has an open order status
                if (status != "Paid")
                {
                    return "Order does not have an open status and can't be dispatched";
                }

                int tracking = Int32.Parse(GetRandomString(8, "number"));
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                //set order status id to shipped
                OrderStatus shippedStatus = _context.OrderStatuses.Where(od => od.Status == "Shipped").FirstOrDefault();
                order.FkOrderStatusId = shippedStatus.PkOrderStatusId;
                order.Tracking = tracking;
                order.ShipDate = today;
                //_context.SaveChanges();

                return "";
            }
            catch (Exception ex)
            {
                return "An unexpected error occured while dispatching order";
            }
        }


        public OrderVM GetOrderById(int orderId)
        {
            var query = _context.Orders.Where(o => o.PkOrderId == orderId).Select(o => new OrderVM
            {
                OrderId = o.PkOrderId,
                OrderDate = o.OrderDate,
                OrderStatus = _context.OrderStatuses
                                   .Where(os => os.PkOrderStatusId == o.FkOrderStatusId)
                                   .Select(os => os.Status)
                                   .FirstOrDefault()
                                   .ToString(),
                BuyerNote = o.BuyerNote,
                OrderDetails = _context.OrderDetails
                                   .Where(od => od.FkOrderId == o.PkOrderId)
                                   .Select(od => new OrderDetail
                                   {
                                       Quantity = od.Quantity,
                                       UnitPrice = od.UnitPrice,
                                       FkSku = _context.ProductSkus
                                           .Where(psku => psku.PkSkuId == od.FkSkuId)
                                           .Select(fsku => new ProductSku
                                           {
                                               Size = fsku.Size,
                                               FkProduct = _context.Products
                                                   .Where(p => p.PkProductId == fsku.FkProductId)
                                                   .FirstOrDefault()
                                           }).FirstOrDefault()
                                   }).ToList(),
                Contact = _context.Contacts
                           .Where(u => u.PkContactId == o.FkContactId)
                           .Include(u => u.Customers)
                           .FirstOrDefault(),
                Discount = _context.Discounts
                           .Where(d => d.PkDiscountCode == o.FkDiscountCode)
                           .FirstOrDefault(),

                OrderTotal = Math.Round(_context.Orders
                       .Join(_context.OrderDetails,
                               o => o.PkOrderId,
                               od => od.FkOrderId,
                               (o, od) => new
                               {
                                   Order = o,
                                   OrderDetail = od
                               })
                       .LeftJoin(_context.Discounts,
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
            }).FirstOrDefault();

            //apply discount to orderTotal
            if (query.Discount != null)
            {
                if (query.Discount.DiscountType.ToLower() == "percent")
                {
                    query.OrderTotal = query.OrderTotal * (1 - query.Discount.DiscountValue / 100);
                }
                else
                {
                    query.OrderTotal = query.OrderTotal - query.Discount.DiscountValue;
                }
            }

            return query;

        }

        public string CancelOrder(int orderId)
        {
            try
            {
                Order order = _context.Orders.Where(o => o.PkOrderId == orderId).FirstOrDefault();
                OrderStatus cancelledStatus = _context.OrderStatuses.Where(os => os.Status == "Cancelled").FirstOrDefault();
                order.FkOrderStatusId = cancelledStatus.PkOrderStatusId;
                //_context.SaveChanges();
                return "";
            }
            catch (Exception ex)
            {
                return "An unexpected error occured";
            }
        }

        public IQueryable<DiscountVM> GetAllDiscounts()
        {
            return _context.Discounts.Select(d =>
                new DiscountVM
                {
                    PkDiscountCode = d.PkDiscountCode,
                    DiscountValue = d.DiscountValue,
                    DiscountType = d.DiscountType,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    IsActive = d.IsActive
                }); ;
        }


        public Discount GetDiscountById(string discountCode)
        {
            return _context.Discounts
                .Where(d => d.PkDiscountCode == discountCode)
                .FirstOrDefault();
        }

        public string CreateDiscount(string discountCode,
                                        decimal discountValue,
                                        string discountType,
                                        DateOnly startDate,
                                        DateOnly endDate
                                       )
        {
            try
            {
                Discount discount = new Discount
                {
                    PkDiscountCode = discountCode,
                    DiscountValue = discountValue,
                    DiscountType = discountType,
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = "1"
                };

                _context.Discounts.Add(discount);
                _context.SaveChanges();
                return "";
            }
            catch (Exception ex)
            {
                return "An unexpected error ocurred while creating discount";
            }
        }

        public string DeactivateDiscount(string discountCode)
        {
            try
            {
                Discount discount = _context.Discounts
                                    .Where(d => d.PkDiscountCode == discountCode)
                                    .FirstOrDefault();

                discount.IsActive = "0";
                _context.SaveChanges();
                return "Discount successfully deactivated";
            }
            catch (Exception ex)
            {
                return "An unexpected error occured while deactivating the discount";
            }

        }

        public string GetRandomString(int size, string type = "number")
        {
            char[] chars;
            if (type == "number")
            {
                chars =
                "1234567890".ToCharArray();
            }
            else
            {
                chars =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            }


            byte[] data = new byte[4 * size];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }
    }
}