using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class CustomerVM
    {
        public string Email { get; set; }

        public Customer Customer { get; set; }
        public Contact? DefaultContact { get; set; }

        public List <Order>?Orders { get; set; }
      
    }
}
