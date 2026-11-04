using System;
using System.Collections.Generic;

namespace Momentum.Domain.Entities;

public partial class Habit
{
    public int HabitId { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? TargetValue { get; set; }

    public string? TargetUnit { get; set; }

    public string FrequencyType { get; set; } = null!;

    public string? DaysOfWeek { get; set; }

    public string? ColorHex { get; set; }

    public string? IconName { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool IsArchived { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();

    public virtual ICollection<StreakFreeze> StreakFreezes { get; set; } = new List<StreakFreeze>();

    public virtual User User { get; set; } = null!;
}

public partial class Habit
{

    public bool IsScheduledForDate(DateOnly date)
    {
        if (date < StartDate) return false;
        if (EndDate.HasValue && date > EndDate.Value) return false;

        if (FrequencyType == "Daily") return true;

        if (FrequencyType == "SpecificDays" && !string.IsNullOrEmpty(DaysOfWeek))
        {
            var dow = (int)date.DayOfWeek;
            return DaysOfWeek.Split(',').Select(int.Parse).Contains(dow);
        }

        return false;
    }
}