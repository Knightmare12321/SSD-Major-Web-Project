using NuGet.Packaging.Signing;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

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

        public IQueryable<OrderVM> GetOrdersByStatus(string orderStatus = "")
        {
            //find order satus record id of the given order status
            int orderStatusId = _context.OrderStatuses
                .Where(os => os.Status == orderStatus)
                .Select(os => os.PkOrderStatusId)
                .FirstOrDefault();

            return _context.Orders.Where(o => orderStatus == "" || o.FkOrderStatusId == orderStatusId).Select(o => new OrderVM
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
                    .Select((order) => order.OrderDetail.Quantity * order.OrderDetail.UnitPrice * (order.Discount != null ? (1 - order.Discount.DiscountValue) : 1))
                    .Sum(), 2)
            });
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
                    return JsonConvert.SerializeObject(
                   new
                   {
                       Success = false,
                       Error = "Order does not have an open status"
                   });
                }

                //set order status id to shipped
                OrderStatus shippedStatus = _context.OrderStatuses.Where(od => od.Status == "Shipped").FirstOrDefault();
                order.FkOrderStatusId = shippedStatus.PkOrderStatusId;
                //_context.SaveChanges();

                return JsonConvert.SerializeObject(new { Success = true, Error = "" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(
                    new
                    {
                        Success = false,
                        Error = $"An unexpected error occured while dispatching order"
                    });
            }
        }


        public OrderVM GetOrderById(int orderId)
        {
            return _context.Orders.Where(o => o.PkOrderId == orderId).Select(o => new OrderVM
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
                    .Select((order) => order.OrderDetail.Quantity * order.OrderDetail.UnitPrice * (order.Discount != null ? (1 - order.Discount.DiscountValue) : 1))
                    .Sum(), 2)
            }).FirstOrDefault();
        }

        public Discount CreateDiscount(string discountCode,
                                        decimal discountValue,
                                        string discountType,
                                        DateOnly startDate,
                                        DateOnly endDate
                                       )
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
            //_context.SaveChanges();
            return discount;
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

    }
}