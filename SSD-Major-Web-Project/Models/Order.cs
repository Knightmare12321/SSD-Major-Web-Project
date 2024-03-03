using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class Order
{
    public int PkOrderId { get; set; }

    public string? FkCustomerId { get; set; }

    public int FkOrderStatusId { get; set; }

    public string? FkDiscountCode { get; set; }

    public int? FkContactId { get; set; }

    public string? TransactionId { get; set; }

    public string? BuyerNote { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly? ShipDate { get; set; }

    public int? Tracking { get; set; }

    public virtual Contact? FkContact { get; set; }

    public virtual Customer? FkCustomer { get; set; }

    public virtual Discount? FkDiscountCodeNavigation { get; set; }

    public virtual OrderStatus FkOrderStatus { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
