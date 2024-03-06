using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class UserVM
    {

        [Required]
        [Display(Name = "Email")]
        public string? Email { get; set; }

    }
}
