using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class CreateProductVM
    {
        [Key]
        public int PkProductId { get; set; }

        public string Name { get; set; } = null!;

        public double Price { get; set; }

        public string Description { get; set; } = null!;

        public string IsActive { get; set; } = null!;

        public byte[]? Image { get; set; }
    }
}
