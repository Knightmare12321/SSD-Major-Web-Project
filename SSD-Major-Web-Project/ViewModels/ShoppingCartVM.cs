using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class ShoppingCartVM
    {

        public string UserId { get; set; }
        public List<Product> Products { get; set; }
        public string CouponCode { get; set; }
        public double Subtotal { get; set; }
        public double ShippingFee { get; set; }
        public double Taxes { get; set; }
        public double GrandTotal { get; set; }

        public byte[]? ImageByteArray { get; set; }

    }
}
