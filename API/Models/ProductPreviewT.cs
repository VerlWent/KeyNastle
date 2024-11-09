using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class ProductPreviewT
{
    public int ProductId { get; set; }

    public int PreviewId { get; set; }

    public string UserLogin { get; set; } = null!;

    public int Id { get; set; }

    public virtual PreviewT Preview { get; set; } = null!;

    public virtual ProductT Product { get; set; } = null!;

    public virtual UserT UserLoginNavigation { get; set; } = null!;
}
