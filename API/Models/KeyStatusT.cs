using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class KeyStatusT
{
    public int Id { get; set; }

    public string KeyStatus { get; set; } = null!;

    public virtual ICollection<KeyStorageT> KeyStorageTs { get; } = new List<KeyStorageT>();
}
