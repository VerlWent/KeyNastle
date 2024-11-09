using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class PreviewT
{
    public int Id { get; set; }

    public string PreviewContent { get; set; } = null!;

    public int PreviewRating { get; set; }

    public virtual ICollection<ProductPreviewT> ProductPreviewTs { get; } = new List<ProductPreviewT>();
}
