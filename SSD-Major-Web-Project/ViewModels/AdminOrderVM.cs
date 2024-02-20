using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class AdminOrderVM
    {
        public ICollection<OrderVM>? AllOrders { get; set; }

        public ICollection<OrderVM>? PendingOrders { get; set; }

        public ICollection<OrderVM>? OpenOrders { get; set; }

        public ICollection<OrderVM>? ShippedOrders { get; set; }

        public ICollection<OrderVM>? DeliveredOrders { get; set; }

    }
}
