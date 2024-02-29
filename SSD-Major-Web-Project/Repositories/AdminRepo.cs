using NuGet.Packaging.Signing;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace SSD_Major_Web_Project.Repositories
{
    public class AdminRepo
    {
        private readonly NovaDbContext _context;

        public AdminRepo(NovaDbContext context)
        {
            _context = context;
        }

        public async Task AddProduct(string name, double price, string description, string isActive, IFormFile imageFile, List<string> sizes)
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



        //public IQueryable<OrderItemVM> GetAllOrderItems()
        //{
        //    return _context.Orders.Join(
        //        _context.OrderDetails,
        //        o => o.PkOrderId,
        //        od => od.FkOrderId,
        //        (o, od) =>
        //        new
        //        {
        //            Order = o,
        //            OrderDetail = od
        //        })
        //        .Join(_context.ProductSkus,
        //            ood => ood.OrderDetail.FkSkuId,
        //            pSku => pSku.PkSkuId,
        //            (ood, pSku) => new
        //            {
        //                ood.Order,
        //                ood.OrderDetail,
        //                ProductSku = pSku
        //            })
        //        .Join(_context.Products,
        //        oodp => oodp.ProductSku.FKproductId,
        //        p => p.PkProductId,
        //        (oodp, p) => new
        //        {
        //            oodp.Order,
        //            oodp.OrderDetail,
        //            oodp.ProductSku,
        //            Product = p
        //        })
        //        .Join(_context.Images,
        //        oodpp => oodpp.Product.PkProductId,
        //        i => i.FkProductId,
        //        (oodpp, i) => new
        //        {
        //            oodpp.Order,
        //            oodpp.OrderDetail,
        //            oodpp.ProductSku,
        //            oodpp.Product,
        //            Image = i
        //        })
        //        .Join(_context.OrderStatuses,
        //        oodppi => oodppi.Order.FkOrderStatusId,
        //        os => os.PkOrderStatusId,
        //        (oodppi, os) => new
        //        {
        //            oodppi.Order,
        //            oodppi.OrderDetail,
        //            oodppi.ProductSku,
        //            oodppi.Product,
        //            oodppi.Image,
        //            OrderStatus = os
        //        })
        //        .Join(_context.Discounts,
        //        oodppio => oodppio.Order.FkDiscountCode,
        //        d => d.PkDiscountCode,
        //        (oodppio, d) => new
        //        {
        //            oodppio.Order,
        //            oodppio.OrderDetail,
        //            oodppio.ProductSku,
        //            oodppio.Product,
        //            oodppio.Image,
        //            oodppio.OrderStatus,
        //            Discount = d
        //        })
        //        .Join(_context.Contacts,
        //        oodppiod => oodppiod.Order.FkContactId,
        //        c => c.PkContactId,
        //        (oodppiod, c) => new
        //        {
        //            oodppiod.Order,
        //            oodppiod.OrderDetail,
        //            oodppiod.ProductSku,
        //            oodppiod.Product,
        //            oodppiod.Image,
        //            oodppiod.OrderStatus,
        //            oodppiod.Discount,
        //            Contact = c
        //        })
        //        .Select(order => new OrderItemVM
        //        {
        //            OrderId = order.Order.PkOrderId,
        //            OrderDate = order.Order.OrderDate,
        //            BuyerNote = order.Order.BuyerNote,
        //            Quantity = order.OrderDetail.Quantity,
        //            Size = order.ProductSku.Size,
        //            ProductName = order.Product.Name,
        //            ProductImage = order.Image.Data,
        //            UnitPrice = order.Product.Price,
        //            Contact = order.Contact,
        //            Discount = order.Discount,
        //            OrderStatus = order.OrderStatus.Status
        //        });
        //}

        public IQueryable<OrderVM> GetAllOrders()
        {
            return _context.Orders.Select(o => new OrderVM
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
                                    FkSku = _context.ProductSkus
                                        .Where(psku => psku.PkSkuId == od.FkSkuId)
                                        .Select(fsku => new ProductSku
                                        {
                                            Size = fsku.Size,
                                            FKproduct = _context.Products
                                                .Where(p => p.PkProductId == fsku.FKproductId)
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
                    .Join(_context.ProductSkus,
                            ood => ood.OrderDetail.FkSkuId,
                            psku => psku.PkSkuId,
                            (ood, psku) => new
                            {
                                ood.Order,
                                ood.OrderDetail,
                                ProductSku = psku
                            })
                    .Join(_context.Products,
                            oodp => oodp.ProductSku.FKproductId,
                            p => p.PkProductId,
                            (oodp, p) => new
                            {
                                oodp.Order,
                                oodp.OrderDetail,
                                oodp.ProductSku,
                                Product = p
                            })
                    .LeftJoin(_context.Discounts,
                            oodpp => oodpp.Order.FkDiscountCode,
                            d => d.PkDiscountCode,
                            (oodpp, d) => new
                            {
                                oodpp.Order,
                                oodpp.OrderDetail,
                                oodpp.ProductSku,
                                oodpp.Product,
                                Discount = d
                            })
                    .Where(order => order.OrderDetail.FkOrderId == o.PkOrderId)
                    .Select((order) => order.OrderDetail.Quantity * order.Product.Price * (order.Discount != null ? (1 - order.Discount.DiscountValue) : 1))
                    .Sum(), 2)
            });
        }

        public IQueryable<OrderVM> GetOrdersByStatus(string orderStatus)
        {
            //find order satus record id of the given order status
            int orderStatusId = _context.OrderStatuses
                .Where(os => os.Status == orderStatus)
                .Select(os => os.PkOrderStatusId)
                .FirstOrDefault();

            return _context.Orders.Where(o => orderStatus == null || o.FkOrderStatusId == orderStatusId).Select(o => new OrderVM
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
                                    FkSku = _context.ProductSkus
                                        .Where(psku => psku.PkSkuId == od.FkSkuId)
                                        .Select(fsku => new ProductSku
                                        {
                                            Size = fsku.Size,
                                            FKproduct = _context.Products
                                                .Where(p => p.PkProductId == fsku.FKproductId)
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
                    .Join(_context.ProductSkus,
                            ood => ood.OrderDetail.FkSkuId,
                            psku => psku.PkSkuId,
                            (ood, psku) => new
                            {
                                ood.Order,
                                ood.OrderDetail,
                                ProductSku = psku
                            })
                    .Join(_context.Products,
                            oodp => oodp.ProductSku.FKproductId,
                            p => p.PkProductId,
                            (oodp, p) => new
                            {
                                oodp.Order,
                                oodp.OrderDetail,
                                oodp.ProductSku,
                                Product = p
                            })
                    .LeftJoin(_context.Discounts,
                            oodpp => oodpp.Order.FkDiscountCode,
                            d => d.PkDiscountCode,
                            (oodpp, d) => new
                            {
                                oodpp.Order,
                                oodpp.OrderDetail,
                                oodpp.ProductSku,
                                oodpp.Product,
                                Discount = d
                            })
                    .Where(order => order.OrderDetail.FkOrderId == o.PkOrderId)
                    .Select((order) => order.OrderDetail.Quantity * order.Product.Price * (order.Discount != null ? (1 - order.Discount.DiscountValue) : 1))
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


    }
}