using NuGet.Packaging.Signing;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Repositories
{
    public class AdminRepository
    {
        private readonly NovaDbContext _context;

        public AdminRepository(NovaDbContext context)
        {
            _context = context;
        }

        public IQueryable<OrderItemVM> GetAllOrderItems()
        {
            return _context.Orders.Join(
                _context.OrderDetails,
                o => o.PkOrderId,
                od => od.FkOrderId,
                (o, od) =>
                new
                {
                    Order = o,
                    OrderDetail = od
                })
                .Join(_context.ProductSkus,
                    ood => ood.OrderDetail.FkSkuId,
                    pSku => pSku.PkSkuId,
                    (ood, pSku) => new
                    {
                        ood.Order,
                        ood.OrderDetail,
                        ProductSku = pSku
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
                .Join(_context.Users,
                oodpp => oodpp.Order.FkUserId,
                u => u.PkUserId,
                (oodpp, u) => new
                {
                    oodpp.Order,
                    oodpp.OrderDetail,
                    oodpp.ProductSku,
                    oodpp.Product,
                    User = u
                })
                .Join(_context.OrderStatuses,
                oodppu => oodppu.Order.FkOrderStatusId,
                os => os.PkOrderStatusId,
                (oodppu, os) => new
                {
                    oodppu.Order,
                    oodppu.OrderDetail,
                    oodppu.ProductSku,
                    oodppu.Product,
                    oodppu.User,
                    OrderStatus = os
                })
                .Join(_context.Discounts,
                oodppuo => oodppuo.Order.FkDiscountCode,
                d => d.PkDiscountCode,
                (oodppuo, d) => new
                {
                    oodppuo.Order,
                    oodppuo.OrderDetail,
                    oodppuo.ProductSku,
                    oodppuo.Product,
                    oodppuo.User,
                    oodppuo.OrderStatus,
                    Discount = d
                })
                .Select(order => new OrderItemVM
                {
                    OrderId = order.Order.PkOrderId,
                    OrderDate = order.Order.OrderDate,
                    BuyerNote = order.Order.BuyerNote,
                    Quantity = order.OrderDetail.Quantity,
                    Size = order.ProductSku.Size,
                    ProductName = order.Product.Name,
                    ProductImage = order.Product.Image,
                    UnitPrice = order.Product.Price,
                    User = order.User,
                    Discount = order.Discount,
                    OrderStatus = order.OrderStatus.Status
                });
        }

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
                User = _context.Users
                        .Where(u => u.PkUserId == o.FkUserId)
                        .FirstOrDefault(),
                Discount = _context.Discounts
                        .Where(d => d.PkDiscountCode == o.FkDiscountCode)
                        .FirstOrDefault(),
                //OrderTotal = o.OrderDetails
                //        .Where(od => od.FkOrderId == o.PkOrderId)
                //        .Select(od => od.Quantity * od.FkSku.FKproduct.Price * (1 - o.FkDiscountCodeNavigation.DiscountValue))
                //        .Sum()
                OrderTotal = 100
            });
        }

        public double GetOrderTotal(int orderId)
        {
            return _context.Orders
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
                    .Join(_context.Discounts,
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
                    .Where(o => o.OrderDetail.FkOrderId == orderId)
                    .Select((o) => o.OrderDetail.Quantity * o.Product.Price * (1 - o.Discount.DiscountValue))
                    .Sum();

        }
    }
}