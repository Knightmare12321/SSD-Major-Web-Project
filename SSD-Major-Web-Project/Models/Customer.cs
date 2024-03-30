using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.Models;

public partial class Customer
{
    [Display(Name = "Customer Id")]
    public string PkCustomerId { get; set; } = null!;
    [Display(Name = "Customer Cotnact Id")]
    public int? FkContactId { get; set; }

    public virtual Contact? FkContact { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
