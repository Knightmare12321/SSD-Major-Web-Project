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

        public List<OrderVM> GetOrders()
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
                .Select(order => new OrderVM
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
                    OrderStatus = order.OrderStatus.OrderStatus1
                }).ToList();
        }
    }
}
