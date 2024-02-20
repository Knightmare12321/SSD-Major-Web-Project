using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class Customer
{
    public string PkCustomerId { get; set; } = null!;

    public int? FkUserTypeId { get; set; }

    public int? FkAddressId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual Address? FkAddress { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
