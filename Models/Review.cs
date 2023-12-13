using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class Review
{
    public string FkUserId { get; set; } = null!;

    public int FkProductId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public virtual Product FkProduct { get; set; } = null!;

    public virtual User FkUser { get; set; } = null!;
}
