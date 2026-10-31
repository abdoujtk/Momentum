using System;
using System.Collections.Generic;

namespace Momentum.Domain.Entities;

public partial class StreakFreeze
{
    public int FreezeId { get; set; }

    public int HabitId { get; set; }

    public DateOnly FreezeDate { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Habit Habit { get; set; } = null!;
}
