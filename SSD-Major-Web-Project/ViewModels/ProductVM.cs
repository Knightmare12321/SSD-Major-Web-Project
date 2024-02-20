﻿using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class ProductVM
    {
        [Key]
        [Display(Name = "ID")]
        public int PkProductId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public double Price { get; set; }

        public string Description { get; set; } = null!;

        [Display(Name = "Is Active")]
        [Required]
        public string IsActive { get; set; } = null!;
    }
}
