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
            _context.Products.Select(u => new ProductVM
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

        // This method is for testing only. Delete this after project completes!
        public Product? GetById(int pkProductId)
        {
            return _context.Products.Where(p => p.PkProductId == pkProductId).FirstOrDefault();
        }

        // This method is for testing only. Delete this after project completes!
        public ProductSku GetSkuById(int id)
        {
            return _context.ProductSkus.Where(p => p.PkSkuId == id).FirstOrDefault();
        }

        public int GetSkuIdById(int id)
        {
            return _context.ProductSkus.Where(p => p.FkProductId == id).FirstOrDefault().PkSkuId;
        }

        public ProductDetailVM? GetByIdAndReviewVM(int pkProductId, List<ReviewVM> reviews)
        {
            ProductVM? productVM = _context.Products
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
