using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class Image
{
    public int PkImageId { get; set; }

    public string FileName { get; set; } = null!;

    public string AltText { get; set; } = null!;

    public byte[] Data { get; set; } = null!;

    public int? FkProductId { get; set; }

    public virtual Product? FkProduct { get; set; }
}
