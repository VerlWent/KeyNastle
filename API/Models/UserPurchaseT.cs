using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class UserPurchaseT
{
    public int Id { get; set; }

    public string? UserLogin { get; set; }

    public int? ProductKeyId { get; set; }

    public DateOnly? DatePurchase { get; set; }

    public virtual ProductKeyT? ProductKey { get; set; }

    public virtual UserT? UserLoginNavigation { get; set; }
}
