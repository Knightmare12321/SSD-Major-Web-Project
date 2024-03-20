using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.ViewModels
{
    public class ReviewVM
    {
        public string FkCustomerId { get; set; } = null!;

        public int FkProductId { get; set; }

        public DateOnly PkReviewDate { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
