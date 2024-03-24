using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class CustomerVM
    {
        public string Email { get; set; }

        public Customer Customer { get; set; }
        public Contact? DefaultContact { get; set; }

        public List <Order>?Orders { get; set; }

        // this is a dictionary that will store the order details for each order
        public Dictionary<int, List<OrderDetail>>? OrdersDetails { get; set; }
        
        // this is a dictionary that will store the product pictures object for each order details, which one order can have more than one order details, one order detail have one product picture file path
        public Dictionary<int, List<Image>>? ProductPictures { get; set; }

    }
}
