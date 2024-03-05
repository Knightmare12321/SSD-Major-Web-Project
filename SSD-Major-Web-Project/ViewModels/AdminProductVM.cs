using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class AdminProductVM
    {
        public int PkProductId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string Description { get; set; } = null!;

        public string IsActive { get; set; } = null!;

        public virtual ICollection<Image> Images { get; set; } = new List<Image>();

        public virtual ICollection<ProductSku> ProductSkus { get; set; } = new List<ProductSku>();

    }
}
