using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class DiscountVM
    {
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Code is required.")]
        public string PkDiscountCode { get; set; } = null!;

        [Display(Name = "Value")]
        [Required(ErrorMessage = "Discount value is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Discount value must be a positive number.")]
        public decimal DiscountValue { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessage = "Discount type is required.")]
        public string DiscountType { get; set; } = null!;

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Start date is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DateNotInPast(ErrorMessage = "Start date cannot be in the past.")]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "End date is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DateNotBefore(nameof(StartDate), ErrorMessage = "End date must be after start date.")]
        public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Display(Name = "Status")]
        public bool IsActive { get; set; }

        public SelectList? DiscountTypes { get; set; }
    }

    public class DateNotInPastAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly date && date < DateOnly.FromDateTime(DateTime.Today))
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }

    public class DateNotBeforeAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public DateNotBeforeAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            if (startDateProperty == null)
            {
                throw new ArgumentException("Property with this name not found", _startDatePropertyName);
            }

            var startDateValue = (DateOnly)startDateProperty.GetValue(validationContext.ObjectInstance, null);
            var endDateValue = (DateOnly)value;

            if (endDateValue < startDateValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
