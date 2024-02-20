using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Repositories
{
    public class ProductRepository
    {
        private readonly NovaDbContext _context;

        public ProductRepository(NovaDbContext context)
        {
            _context = context;
        }

        public void AddProduct(string name, double price, string description, string isActive, byte[]? image, List<string> sizes)
        {
            try
            {
                List<ProductSku> productSkus = new List<ProductSku>();
                for (int i = 0; i < sizes.Count; i++)
                {
                    productSkus.Add(new ProductSku() { Size = sizes[i] });

                }
                Product product = new Product() { Name = name, Price = price, Description = description, IsActive = isActive, Image = image, ProductSkus = productSkus };
                _context.Products.Add(product);
                //_context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
    }
}
