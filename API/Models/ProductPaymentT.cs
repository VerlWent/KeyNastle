using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class ProductPaymentT
{
    public int ProductId { get; set; }

    public int PaymentId { get; set; }

    public int Id { get; set; }

    public virtual PaymentTypeT Payment { get; set; } = null!;

    public virtual ProductT Product { get; set; } = null!;
}
