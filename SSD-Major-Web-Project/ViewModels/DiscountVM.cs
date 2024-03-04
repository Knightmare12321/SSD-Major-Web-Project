using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class DiscountVM
    {
        [Display(Name = "Code")]
        public string PkDiscountCode { get; set; } = null!;

        [Display(Name = "Value")]
        public decimal DiscountValue { get; set; }

        [Display(Name = "Type")]
        public string DiscountType { get; set; } = null!;

        [Display(Name = "Start Date")]
        public DateOnly StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateOnly EndDate { get; set; }

        [Display(Name = "Status")]
        public string IsActive { get; set; } = null!;
    }
}
