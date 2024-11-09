using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class UserT
{
    public string Login { get; set; } = null!;

    public string NickName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateOnly RegistrationDate { get; set; }

    public string? Salt { get; set; }

    public int RoleId { get; set; }

    public string? Certificate { get; set; }

    public virtual ICollection<ProductPreviewT> ProductPreviewTs { get; } = new List<ProductPreviewT>();

    public virtual RoleT? Role { get; set; }

    public virtual ICollection<UserBusketT> UserBusketTs { get; } = new List<UserBusketT>();

    public virtual ICollection<UserProductT> UserProductTs { get; } = new List<UserProductT>();

    public virtual ICollection<UserPurchaseT> UserPurchaseTs { get; } = new List<UserPurchaseT>();
}
