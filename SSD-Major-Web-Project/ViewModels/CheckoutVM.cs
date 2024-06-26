﻿using Microsoft.Owin.BuilderProperties;
using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class CheckoutVM
    {
        public OrderVM Order { get; set; }

        // ShoppingCart pass info with entity Product, ProductSku,Image
        public ShoppingCartVM ShoppingCart { get; set; }
     
        [Required(ErrorMessage = "Email cannot be empty.")]
        public string DeliveryContactEmail { get; set; } = null!;

        public string TransactionId { get; set; }

        public List<Contact> UserAddresses { get; set; }

    }
}
