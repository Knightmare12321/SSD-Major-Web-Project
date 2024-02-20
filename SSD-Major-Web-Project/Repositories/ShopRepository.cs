using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Repositories
{
    public class ShopRepository
    {
        private readonly NovaDbContext _context;

        public ShopRepository(NovaDbContext context)
        {
            _context = context;
        }

        //public void AddOrder(int pkOrderId, string fkUserId, int fkORderStatusId, string fkDiscountCode, int fkAdressId, string transactionId, string buyerNote, DateOnly orderDate)
        //{
        //    try
        //    {
        //        List<ProductSku> productSkus = new List<ProductSku>(){ SkuId = skuId};
        //        _context.Order.Add(order);
        //        _context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }

        //}

        // calculate taxes
        public double CalculateSubtotal(DbSet<Product> products)
        {
            double subtotal = 0;

            foreach (var product in products)
            {
                subtotal += product.Price;
            }

            return subtotal;
        }

        // calculate taxes
        public double CalculateTaxes(double subtotal)
        {
            double taxes = 0;

            // change tax rates
            const double TAX_RATES = 0.12;
            // only save 2 decimal places and add zero if needed
            taxes = System.Math.Round(subtotal * TAX_RATES, 2);
            
            return taxes;
        }

        // calculate grand total
        public double CalculateGrandTotal(double subtotal, double taxes, double shippingFee)
        {
            double grandTotal = 0;

            grandTotal = subtotal + taxes + shippingFee;

            return grandTotal;
        }

    }
}
