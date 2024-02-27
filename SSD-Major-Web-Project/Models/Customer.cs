using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class Customer
{
    public string PkCustomerId { get; set; } = null!;

    public int? FkUserTypeId { get; set; }

    public int? FkContactId { get; set; }

    public virtual Contact? FkContact { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
