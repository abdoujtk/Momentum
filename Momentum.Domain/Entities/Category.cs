using System;
using System.Collections.Generic;

namespace Momentum.Domain.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string ColorHex { get; set; } = null!;

    public string IconName { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public virtual ICollection<Habit> Habits { get; set; } = new List<Habit>();
}
