using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class OrderVM
    {
        public int OrderId { get; set; }
        public DateOnly OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string? BuyerNote { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public byte[]? ProductImage { get; set; }
        public string Size { get; set; }
        public User User { get; set; }
        public Discount? Discount { get; set; }
    }
}
