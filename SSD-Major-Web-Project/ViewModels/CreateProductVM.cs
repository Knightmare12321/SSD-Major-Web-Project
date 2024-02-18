using SSD_Major_Web_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class CreateProductVM
    {
        [Key]
        public int PkProductId { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public char IsActive { get; set; } = '1';

        public byte[]? Image { get; set; }

        public List<string> Sizes { get; set; }
    }
}
