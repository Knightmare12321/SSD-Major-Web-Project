namespace SSD_Major_Web_Project.Models;

public partial class Product
{
    public int PkProductId { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public string Description { get; set; } = null!;

    public string IsActive { get; set; } = null!;

    public byte[]? Image { get; set; }

    public virtual ICollection<ProductSku> ProductSkus { get; set; } = new List<ProductSku>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
