using System;
using System.Collections.Generic;

namespace Momentum.Domain.Entities;

public partial class CheckIn
{
    public int CheckInId { get; set; }

    public int HabitId { get; set; }

    public DateOnly CheckInDate { get; set; }

    public decimal? ActualValue { get; set; }

    public bool IsCompleted { get; set; }

    public int? MoodId { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Habit Habit { get; set; } = null!;

    public virtual Mood? Mood { get; set; }
}
