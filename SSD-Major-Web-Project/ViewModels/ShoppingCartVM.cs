using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class ShoppingCartVM
    {
        // nullable UserId
        public string? UserId { get; set; } 
        public List<Product> Products { get; set; }
        public string? CouponCode { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Subtotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ShippingFee { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Taxes { get; set; }
        // [DisplayFormat(DataFormatString = "{0:C}")]

        public double GrandTotal { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }
        //public byte[]? ImageByteArray { get; set; }

    }
}
