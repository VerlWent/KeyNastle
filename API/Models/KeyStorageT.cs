using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class KeyStorageT
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public int StatusId { get; set; }

    public virtual ICollection<ProductKeyT> ProductKeyTs { get; } = new List<ProductKeyT>();

    public virtual KeyStatusT Status { get; set; } = null!;
}
