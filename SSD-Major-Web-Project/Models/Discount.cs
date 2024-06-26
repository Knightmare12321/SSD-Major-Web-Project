﻿using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class Discount
{
    public string PkDiscountCode { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public string DiscountType { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
