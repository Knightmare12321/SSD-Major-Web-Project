﻿using SSD_Major_Web_Project.Models;
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
                IsActive = u.IsActive,
                ImageByteArray = u.Image
            });
            return products;
        }

        public ProductDetailVM? GetById(int pkProductId) {
            ProductVM? productVM = _context.Products
                .Where(u => u.PkProductId == pkProductId)
                .Select(u => new ProductVM
                {
                    PkProductId = u.PkProductId,
                    Name = u.Name,
                    Price = u.Price,
                    Description = u.Description,
                    IsActive = u.IsActive,
                    ImageByteArray = u.Image
                })
                .FirstOrDefault();
            if (productVM == null) { return null; }
            IEnumerable<String> sizes = _context.ProductSkus
                .Where(u => u.FKproductId == pkProductId)
                .Select(x => x.Size)
                .Distinct();
            ProductDetailVM productDetailVM = new ProductDetailVM
            {
                PkProductId = pkProductId,
                Name = productVM.Name,
                Price = productVM.Price,
                Description = productVM.Description,
                IsActive = productVM.IsActive,
                ImageByteArray = productVM.ImageByteArray,
                Sizes = sizes.ToList()
            };
            return productDetailVM;
        }
    }
}