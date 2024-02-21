namespace SSD_Major_Web_Project.Models;

public partial class ProductSku
{
    public int PkSkuId { get; set; }

    public int? FKproductId { get; set; }

    public string Size { get; set; } = null!;

    public virtual Product? FKproduct { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
