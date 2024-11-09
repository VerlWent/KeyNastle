using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class RoleT
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<UserT> UserTs { get; } = new List<UserT>();
}
