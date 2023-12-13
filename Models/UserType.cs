using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class UserType
{
    public int PkUserTypeId { get; set; }

    public string UserType1 { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
