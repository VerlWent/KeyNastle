using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class UserBusketT
{
    public int Id { get; set; }

    public string UserLogin { get; set; } = null!;

    public int ProductId { get; set; }

    public virtual ProductT Product { get; set; } = null!;

    public virtual UserT UserLoginNavigation { get; set; } = null!;
}
