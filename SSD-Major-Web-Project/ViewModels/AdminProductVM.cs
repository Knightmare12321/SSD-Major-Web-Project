using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class AdminProductVM
    {
        [Display(Name = "Product Id")]
        public int PkProductId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string Description { get; set; } = null!;

        public bool IsActive { get; set; }

        public virtual ICollection<Image> Images { get; set; } = new List<Image>();

        public virtual ICollection<ProductSku> ProductSkus { get; set; } = new List<ProductSku>();

    }
}
