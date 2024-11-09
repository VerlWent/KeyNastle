using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class ProductGenreT
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int GenreId { get; set; }

    public virtual GenreT Genre { get; set; } = null!;

    public virtual ProductT Product { get; set; } = null!;
}
