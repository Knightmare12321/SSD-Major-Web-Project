using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class ShoppingCartVM
    {
        // nullable UserId
        public string? UserId { get; set; }
        //public List<Product> Products { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }
        public List<Product> Products { get; set; }
        public List<ProductVM> ProductVMs { get; set; }
        public string? CouponCode { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Subtotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ShippingFee { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Taxes { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]

        public decimal GrandTotal { get; set; }

        //for paypal payment
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; } = "$";
        //public byte[]? ImageByteArray { get; set; }

    }


   
}
