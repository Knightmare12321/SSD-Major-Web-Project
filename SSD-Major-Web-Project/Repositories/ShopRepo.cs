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


        // create new contact for customer
        public Tuple<string, int> AddContact(Contact contact)
        {
            // Placeholder to return message and contact ID
            string message = string.Empty;
            int contactId = 0;

            try
            {
                _context.Add(contact);
                _context.SaveChanges();

                // Retrieve the contact ID
                contactId = contact.PkContactId;

                message = "";
            }
            catch (System.Exception ex)
            {
                message = $"Error adding new contact: {ex.Message}";

                // Log error
                Console.WriteLine(ex.Message);
            }

            return Tuple.Create(message, contactId);
        }

       

        // create new order details
        public string AddOrderDetails(CheckoutVM checkoutVMentity, int orderId)
        {
            // Placeholder to return message
            string message = string.Empty;
            try
            {
                foreach (var product in checkoutVMentity.ShoppingCart.ShoppingCartItems)
                {
                    // Retrieve the productId and get price by inqury the SkuId
                    var sku = _context.ProductSkus.FirstOrDefault(s => s.PkSkuId == product.SkuId);
                    if (sku != null)
                    {
                           var parentProduct = _context.Products.FirstOrDefault(p => p.PkProductId == sku.FkProductId);

                        if (parentProduct == null)
                        {
                            message = $"Error adding new order details: No Price found for the SkuId {checkoutVMentity.Order.OrderId}";
                            return message;
                        }
                        OrderDetail orderDetail = new()
                        {
                            FkOrderId = orderId,
                            FkSkuId = product.SkuId,
                            Quantity = product.Quantity,
                            UnitPrice = parentProduct.Price,
                        };

                        _context.Add(orderDetail);
                    }
                }

                // Save all order details to the database in a single transaction
                _context.SaveChanges();

                message = "";
            }
            catch (Exception ex)
            {
                message = $"Error adding new order details: {checkoutVMentity.Order.OrderId}";

                // Log error
                Console.WriteLine(ex.Message);
            }
            return message;
        }
        public string AddOrder(CheckoutVM checkoutVMentity, int contactId)
        {
            string message = string.Empty;
            try
            {
                Order order = new Order();

                if (checkoutVMentity.Order != null)
                {
                    order.FkCustomerId = checkoutVMentity.DeliveryContactEmail;

                    // Assign FkOrderStatusId based on OrderStatus value
                    order.FkOrderStatusId = checkoutVMentity.Order.OrderStatus switch
                    {
                        // FkOrderStatusId 
                        // 1 : Pending
                        // 2 : Paid
                        // 3 : Shipped
                        // 4 : Delivered
                        // 5 : Cancelled
                        "Pending" => 1,
                        "Paid" => 2,
                        "Shipped" => 3,
                        "Delivered" => 4,
                        "Cancelled" => 5,
                        _ => 0
                    };

                    order.FkDiscountCode = checkoutVMentity.Order.Discount?.PkDiscountCode;
                    order.BuyerNote = checkoutVMentity.Order.BuyerNote;
                    order.OrderDate = checkoutVMentity.Order.OrderDate;
                
                }

                

                order.FkContactId = contactId;
                // Null is duplicated, using a fake transactionId here
                order.TransactionId = contactId.ToString();
    
                //order.TransactionId = checkoutVMentity.TransactionId;
                order.ShipDate = null;
                order.Tracking = null;

                // if customer is not logged in, add a new customer
                if (!IsCustomerExist(checkoutVMentity.DeliveryContactEmail))
                {
                    Customer customer = new Customer
                    {
                        PkCustomerId = checkoutVMentity.DeliveryContactEmail,
                        FkContactId = contactId,
                        
                    };

                    _context.Add(customer);
                    _context.SaveChanges();
                    
                }
                

                _context.Add(order);
                _context.SaveChanges();

                message = "";
            }
            catch (Exception ex)
            {
                message = $"Error placing new order: {checkoutVMentity.Order?.OrderId}";

                // Log error
                Console.WriteLine(ex.Message);
            }

            return message;
        }

        // check if the customer has an account
        public bool IsCustomerExist(string email)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.PkCustomerId == email);
            if (customer != null)
            {
                return true;
            }
            return false;
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
