namespace SSD_Major_Web_Project.Models;

public partial class Review
{
    public string FkCustomerId { get; set; } = null!;

    public int FkProductId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public virtual Customer FkCustomer { get; set; } = null!;

    public virtual Product FkProduct { get; set; } = null!;
}
