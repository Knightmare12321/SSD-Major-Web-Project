using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class OrderVM
    {
        public int OrderId { get; set; }
        public DateOnly OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string? BuyerNote { get; set; }
        public Discount? Discount { get; set; }
        public Customer Customer { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public double OrderTotal { get; set; }
    }
}
