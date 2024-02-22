namespace SSD_Major_Web_Project.Models;

public partial class Discount
{
    public string PkDiscountCode { get; set; } = null!;

    public double DiscountValue { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
