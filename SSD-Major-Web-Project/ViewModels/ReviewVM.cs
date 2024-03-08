namespace SSD_Major_Web_Project.ViewModels
{
    public class ReviewVM
    {
        public string FkCustomerId { get; set; } = null!;

        public DateOnly PkReviewDate { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
