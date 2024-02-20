using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class ShoppingCartVM
    {

        public string UserId { get; set; }
        public List<Product> Products { get; set; }
        public string CouponCode { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Taxes { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
