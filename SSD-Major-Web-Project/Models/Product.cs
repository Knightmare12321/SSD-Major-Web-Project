using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD_Major_Web_Project.Models;

public partial class Product
{
    public int PkProductId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<ProductSku> ProductSkus { get; set; } = new List<ProductSku>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
