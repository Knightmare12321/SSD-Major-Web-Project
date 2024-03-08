using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.ViewModels
{
    public class ProductDetailWithReviewVM
    {
        public ProductDetailVM _productDetailVM;
        public List<Review> _reviews;

        public ProductDetailWithReviewVM(ProductDetailVM productDetailVM, List<Review> reviews)
        {
            _productDetailVM = productDetailVM;

        }
    }
}
