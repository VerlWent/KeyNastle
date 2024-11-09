using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class SellerT
{
    public string Login { get; set; } = null!;

    public string NameShop { get; set; } = null!;

    public decimal? Rating { get; set; }

    public string Certificate { get; set; } = null!;

    public virtual UserT LoginNavigation { get; set; } = null!;

    public virtual ICollection<ProductT> ProductTs { get; } = new List<ProductT>();
}
