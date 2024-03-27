using EllipticCurve.Utils;
using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class ProductDetailVM
    {
        [Key]
        [Display(Name = "ID")]
        public int PkProductId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        public string Description { get; set; } = null!;

        [Display(Name = "Is Active")]
        [Required]
        public bool IsActive { get; set; }

        public List<byte[]>? ImageByteArray { get; set; }

        public List<int> ProductSkuIDs { get; set; }

        public List<System.String>? Sizes { get; set; }

        public List<ReviewVM> Reviews { get; set; }
    }
}
