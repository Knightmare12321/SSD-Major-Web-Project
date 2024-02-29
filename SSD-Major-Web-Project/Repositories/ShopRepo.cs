using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Interface;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Repositories
{
    public class ShopRepo
    {
        private readonly NovaDbContext _context;

        public ShopRepo(NovaDbContext context)
        {
            _context = context;
        }

        //public string AddOrder(CheckoutVM checkoutEntity)
        //{
        //    // place holder to return message
        //    string message = string.Empty;
        //    try
        //    {
        //        _context.Add(checkoutEntity);
        //        _context.SaveChanges();
        //        message = "Order has been placed, you will get an email confirmation";

        //    }
        //    catch (Exception ex)
        //    {
        //        message = $" Error placeing new order: {checkoutEntity.Order.OrderId}";
        //    }
        //    return message;


        //}
        // calculate taxes
        public decimal CalculateSubtotal(List<Product> products)
        {
            decimal subtotal = 0;

            foreach (var product in products)
            {
                subtotal += product.Price;
            }
            return subtotal;
        }

        // calculate taxes
        public decimal CalculateTaxes(decimal subtotal)
        {
            decimal taxes = 0;

            // change tax rates
            const decimal TAX_RATES = 0.12m;
            // only save 2 decimal places and add zero if needed
            taxes = System.Math.Round(subtotal * TAX_RATES, 2);

            return taxes;
        }

        // calculate grand total
        public decimal CalculateGrandTotal(decimal subtotal, decimal taxes, decimal shippingFee)
        {
            decimal grandTotal = 0;

            grandTotal = subtotal + taxes + shippingFee;

            return grandTotal;
        }






    }
}
