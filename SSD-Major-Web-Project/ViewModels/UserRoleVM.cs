using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class UserRoleVM
    {

        [Required]
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string? Email { get; set; }


    }
}
