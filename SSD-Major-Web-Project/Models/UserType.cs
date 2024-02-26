using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class UserType
{
    public int PkUserTypeId { get; set; }

    public string UserType1 { get; set; } = null!;

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
