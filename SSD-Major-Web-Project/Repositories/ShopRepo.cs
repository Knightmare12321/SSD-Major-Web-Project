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

        public string AddOrder(CheckoutVM checkoutEntity)
        {
            // Placeholder to return message
            string message = string.Empty;
            try
            {
                Order order = new Order
                {
                    // Entity attribute mapping
                    FkCustomerId = checkoutEntity.DeliveryContactEmail,

                    // FkOrderStatusId 
                    // 1 : Pending
                    // 2 : Paid
                    // 3 : Shipped
                    // 4 : Delivered
                    // 5 : Cancelled

                    // Use case to determine which status are we using and assign status id accordingly
                    FkOrderStatusId = checkoutEntity.Order.OrderStatus switch
                    {
                        "Pending" => 1,
                        "Paid" => 2,
                        "Shipped" => 3,
                        "Delivered" => 4,
                        "Cancelled" => 5,
                        _ => 0
                    },
                    FkDiscountCode = checkoutEntity.Order.Discount.PkDiscountCode,
                    FkContactId = checkoutEntity.Order.Contact.PkContactId,
                    TransactionId = checkoutEntity.TransactionId,
                    BuyerNote = checkoutEntity.Order.BuyerNote,
                    OrderDate = checkoutEntity.Order.OrderDate,
                };

                _context.Add(order);
                _context.SaveChanges();
                message = "Order has been placed, you will get an email confirmation shortly.";
            }
            catch (Exception ex)
            {
                message = $"Error placing new order: {checkoutEntity.Order.OrderId}";
            }
            return message;
        }



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
