using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class ProductKeyT
{
    public int ProductId { get; set; }

    public int KeyId { get; set; }

    public int Id { get; set; }

    public virtual KeyStorageT Key { get; set; } = null!;

    public virtual ProductT Product { get; set; } = null!;

    public virtual ICollection<UserPurchaseT> UserPurchaseTs { get; } = new List<UserPurchaseT>();
}
