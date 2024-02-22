using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;


namespace SSD_Major_Web_Project.ViewModels
{
    public class OrderVM
    {
        [Key]
        [Required]
        public int OrderId { get; set; }

        [Required]
        public DateOnly OrderDate { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        public string? BuyerNote { get; set; }

        public Discount? Discount { get; set; }

        [Required]
        public Customer Customer { get; set; }

        [Required]
        public List<OrderDetail> OrderDetails { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double OrderTotal { get; set; }
    }
}
