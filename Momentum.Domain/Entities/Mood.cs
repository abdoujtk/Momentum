using System;
using System.Collections.Generic;

namespace Momentum.Domain.Entities;

public partial class Mood
{
    public int MoodId { get; set; }

    public string Name { get; set; } = null!;

    public string? Emoji { get; set; }

    public byte Score { get; set; }

    public string ColorHex { get; set; } = null!;

    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
}
