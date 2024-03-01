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
        public string IsActive { get; set; } = null!;

        public byte[]? ImageByteArray { get; set; }

        public List<String>? Sizes { get; set; }
    }
}
