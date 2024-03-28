using Microsoft.EntityFrameworkCore;
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
            _context.Products
            .Include(p => p.Images)
            .Select(u => new ProductVM
            {
                PkProductId = u.PkProductId,
                Name = u.Name,
                Price = u.Price,
                Description = u.Description,
                IsActive = u.IsActive,
                ImageByteArray = u.Images.FirstOrDefault().Data
            });
            return products;
        }

        public IEnumerable<ProductVM> GetAllActive()
        {
            IEnumerable<ProductVM> products =
            _context.Products
            .Include(p => p.Images)
            .Where(u => u.IsActive)
            .Select(u => new ProductVM
            {
                PkProductId = u.PkProductId,
                Name = u.Name,
                Price = u.Price,
                Description = u.Description,
                IsActive = u.IsActive,
                ImageByteArray = u.Images.FirstOrDefault().Data
            });
            return products;
        }

        public IEnumerable<ProductVM> GetAllActiveWithPages(int page, int itemsPerPage) {
            int skipCount = (page - 1) * itemsPerPage;
            return _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.PkProductId)
            .Skip(skipCount)
            .Take(itemsPerPage)
            .Include(p => p.Images)
            .Select(p => new ProductVM
            { 
                PkProductId = p.PkProductId,
                Name = p.Name,
                Price = p.Price,
                ImageByteArray = p.Images.FirstOrDefault().Data
            })
            .ToList();
        }

        // This method is for testing only. Delete this after project completes!
        public Product? GetById(int pkProductId)
        {
            return _context.Products
                .Include(p => p.Images)
                .Where(p => p.PkProductId == pkProductId)
                .FirstOrDefault();
        }

        // This method is for testing only. Delete this after project completes!
        public ProductSku GetSkuById(int id)
        {
            return _context.ProductSkus
                .Where(p => p.PkSkuId == id)
                .FirstOrDefault();
        }

        public int GetSkuIdById(int id)
        {
            return _context.ProductSkus
                .Where(p => p.FkProductId == id)
                .Select(p => p.PkSkuId)
                .FirstOrDefault();
        }

        public ProductDetailVM? GetByIdAndReviewVM(int pkProductId, List<ReviewVM> reviews)
        {
            ProductVM? productVM = _context.Products
                .Include(p => p.Images)
                .Where(u => u.PkProductId == pkProductId)
                .Select(u => new ProductVM
                {
                    PkProductId = u.PkProductId,
                    Name = u.Name,
                    Price = u.Price,
                    Description = u.Description,
                    IsActive = u.IsActive,
                    ImageByteArray = u.Images.FirstOrDefault().Data
                })
                .FirstOrDefault();
            if (productVM == null) { return null; }
            List<byte[]> imageByteArray = _context.Images
                .Where(u => u.FkProductId == pkProductId)
                .Select(u => u.Data)
                .ToList();
            var sizeWithIDs = _context.ProductSkus
                .Where(u => u.FkProductId == pkProductId)
                .Select(x => new
                {
                    skuID = x.PkSkuId,
                    size = x.Size
                })
                .Distinct();
            //IEnumerable<String> sizes = _context.ProductSkus
            //    .Where(u => u.FkProductId == pkProductId)
            //    .Select(x => x.Size)
            //    .Distinct();
            List<int> skuIDs = new List<int>();
            foreach (var item in sizeWithIDs) { skuIDs.Add(item.skuID); }
            List<String> sizes = new List<String>();
            foreach (var item in sizeWithIDs) { sizes.Add(item.size); }
            ProductDetailVM productDetailVM = new ProductDetailVM
            {
                PkProductId = pkProductId,
                Name = productVM.Name,
                Price = productVM.Price,
                Description = productVM.Description,
                IsActive = productVM.IsActive,
                ImageByteArray = imageByteArray,
                ProductSkuIDs = skuIDs,
                Sizes = sizes,
                Reviews = reviews
            };
            return productDetailVM;
        }
    }
}
