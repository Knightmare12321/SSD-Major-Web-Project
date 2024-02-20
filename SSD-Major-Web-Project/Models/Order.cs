namespace SSD_Major_Web_Project.Models;

public partial class Order
{
    public int PkOrderId { get; set; }

    public string? FkUserId { get; set; }

    public int FkOrderStatusId { get; set; }

    public string? FkDiscountCode { get; set; }

    public int? FkAddressId { get; set; }

    public string? TransactionId { get; set; }

    public string? BuyerNote { get; set; }

    public DateOnly OrderDate { get; set; }

    public virtual Address? FkAddress { get; set; }

    public virtual Discount? FkDiscountCodeNavigation { get; set; }

    public virtual OrderStatus FkOrderStatus { get; set; } = null!;

    public virtual User? FkUser { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
