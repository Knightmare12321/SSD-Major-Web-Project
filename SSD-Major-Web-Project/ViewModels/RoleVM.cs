using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class RoleVM
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }

    }
}
