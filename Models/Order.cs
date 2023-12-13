﻿using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class Order
{
    public int PkOrderId { get; set; }

    public string? FkUserId { get; set; }

    public int FkOrderStatusId { get; set; }

    public string? FkDiscountCode { get; set; }

    public string? TransactionId { get; set; }

    public string? BuyerNote { get; set; }

    public DateTime OrderDate { get; set; }

    public virtual Discount? FkDiscountCodeNavigation { get; set; }

    public virtual OrderStatus FkOrderStatus { get; set; } = null!;

    public virtual User? FkUser { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
