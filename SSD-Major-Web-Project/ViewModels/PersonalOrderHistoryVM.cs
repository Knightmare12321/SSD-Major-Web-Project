using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class PersonalOrderHistoryVM
    {
        public int OrderId { get; set; }

        public DateOnly OrderDate { get; set; }

        public DateOnly? ShipDate { get; set; }

        public int? Tracking { get; set; }
        public string? TransactionId { get; set; }

        public string OrderStatus { get; set; }

        public string? BuyerNote { get; set; }

        public Discount? Discount { get; set; }

        public Contact Contact { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }

        public decimal OrderTotal { get; set; }
    }
}
