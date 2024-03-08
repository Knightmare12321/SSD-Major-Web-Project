using System.ComponentModel.DataAnnotations;

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
        public decimal Price { get; set; }

        public string Description { get; set; } = null!;

        [Display(Name = "Is Active")]
        [Required]
        public bool IsActive { get; set; }
        public byte[]? ImageByteArray { get; set; }
        public List<ReviewVM> Reviews { get; set; }
    }
}
