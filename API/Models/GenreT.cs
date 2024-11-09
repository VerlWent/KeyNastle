using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class GenreT
{
    public int Id { get; set; }

    public string GenreName { get; set; } = null!;

    public virtual ICollection<ProductGenreT> ProductGenreTs { get; } = new List<ProductGenreT>();
}
