using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class OrderStatus
{
    public int PkOrderStatusId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
