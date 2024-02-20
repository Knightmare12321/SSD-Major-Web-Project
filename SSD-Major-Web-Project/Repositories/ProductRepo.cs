using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Repositories
{
    public class ProductRepo
    {
        private readonly NovaDbContext _context;
        public ProductRepo(NovaDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductVM> GetAll()
        {
            IEnumerable<ProductVM> products =
            _context.Products.Select(u => new ProductVM {
                PkProductId = u.PkProductId,
                Name = u.Name,
                Price = u.Price,
                Description = u.Description,
                IsActive = u.IsActive
            });
            return products;
        }
    }
}
