using Microsoft.Extensions.Hosting;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;
using System.Linq;

namespace SSD_Major_Web_Project.Repositories
{
    public class ReviewRepo
    {

        private readonly NovaDbContext _context;

        public ReviewRepo(NovaDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Review> GetAll()
        {
            return _context.Reviews;
        }

        public Review? GetById(DateOnly date)
        {
            return _context.Reviews
                            .FirstOrDefault(r => r.PkReviewDate == date);
        }

        public List<Review> GetReviewsForProduct(int id)
        {
            return _context.Reviews.Where(r => r.FkProductId == id).ToList();
        }

        public string Add(ReviewVM entity)
        {
            string message = string.Empty;
            Review review = new Review
            {
                FkCustomerId = entity.FkCustomerId,
                PkReviewDate = DateOnly.FromDateTime(DateTime.Now),
                Rating = entity.Rating,
                FkProductId = entity.FkProductId,
                Comment = entity.Comment,
            };
            try
            {
                _context.Reviews.Add(review);
                _context.SaveChanges();
                message = $"Review for {entity.FkProductId} saved successfully";
            }
            catch (Exception e)
            {
                message = $" Error saving review for {entity.FkProductId}: {e}";
            }
            return message;
        }

        public string Update(Review entity)
        {
            string message = string.Empty;
            try
            {
                Review review = GetById(entity.PkReviewDate) ?? new Review();
                review.FkCustomerId = entity.FkCustomerId;
                review.FkProductId = entity.FkProductId;
                review.Rating = entity.Rating;
                review.Comment = entity.Comment;
                _context.SaveChanges();
                message = $"Review for {entity.FkProductId} updated successfully";
            }
            catch (Exception e)
            {
                message = $" Error updating {entity.FkProductId}: {e.Message}";
            }
            return message;
        }

        public string Delete(DateOnly date)
        {
            string message = string.Empty;
            try
            {
                Review review = GetById(date);
                _context.Remove(review);
                _context.SaveChangesAsync();
                message = $"Review for {review.FkProductId} deleted successfully";
            }
            catch (Exception e)
            {
                message = $" Error deleting Review-{date}: {e.Message}";
            }
            return message;
        }
    }
}
