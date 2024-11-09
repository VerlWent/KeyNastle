using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class ProductT
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Image { get; set; } = null!;

    public decimal Price { get; set; }

    public DateOnly DateAdded { get; set; }

    public virtual ICollection<ProductGenreT> ProductGenreTs { get; } = new List<ProductGenreT>();

    public virtual ICollection<ProductKeyT> ProductKeyTs { get; } = new List<ProductKeyT>();

    public virtual ICollection<ProductPreviewT> ProductPreviewTs { get; } = new List<ProductPreviewT>();

    public virtual ICollection<UserBusketT> UserBusketTs { get; } = new List<UserBusketT>();

    public virtual ICollection<UserProductT> UserProductTs { get; } = new List<UserProductT>();
}
