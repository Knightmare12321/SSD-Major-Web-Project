using System;
using System.Collections.Generic;

namespace SSD_Major_Web_Project.Models;

public partial class OrderDetail
{
    public int FkOrderId { get; set; }

    public int FkSkuId { get; set; }

    public int Quantity { get; set; }

    public double UnitPrice { get; set; }

    public virtual Order FkOrder { get; set; } = null!;

    public virtual ProductSku FkSku { get; set; } = null!;
}
