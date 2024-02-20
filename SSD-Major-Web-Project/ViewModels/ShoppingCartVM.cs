using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class ShoppingCartVM
    {
        [Key]
        public string TempOrderId { get; set; }
        public string UserId { get; set; }
        public List<Product> Products { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Taxes { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
