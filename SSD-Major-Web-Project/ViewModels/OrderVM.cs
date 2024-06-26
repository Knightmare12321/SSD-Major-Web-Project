﻿using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;


namespace SSD_Major_Web_Project.ViewModels
{
    public class OrderVM
    {
        [Key]
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [Required]
        public DateOnly OrderDate { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        public string? BuyerNote { get; set; }

        public Discount? Discount { get; set; }

        public DateOnly? ShipDate { get; set; }
        public int? Tracking { get; set; }

        [Required]
        public Contact Contact { get; set; }

        [Required]
        public List<OrderDetail> OrderDetails { get; set; }

        [Required]
        public decimal OrderTotal { get; set; }
    }
}
