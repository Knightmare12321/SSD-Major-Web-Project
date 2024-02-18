using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class AdminOrderVM
    {
        public ICollection<OrderVM>? OpenOrders { get; set; }
    }
}
