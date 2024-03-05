using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class CreateProductVM
    {
        [Key]
        public int PkProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        public string IsActive { get; set; } = "1";

        [Required]
        public List<IFormFile> Images { get; set; }

        [Required]
        public List<string> Sizes { get; set; }
    }
}
