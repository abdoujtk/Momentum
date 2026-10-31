using System;
using System.Collections.Generic;

namespace Momentum.Domain.Entities;

public partial class Quote
{
    public int QuoteId { get; set; }

    public string Text { get; set; } = null!;

    public string? Author { get; set; }

    public bool IsActive { get; set; }
}
