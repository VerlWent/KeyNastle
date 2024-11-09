using System;
using System.Collections.Generic;

namespace API1.Models;

public partial class PaymentTypeT
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public virtual ICollection<ProductPaymentT> ProductPaymentTs { get; } = new List<ProductPaymentT>();
}
