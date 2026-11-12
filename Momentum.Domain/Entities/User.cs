using System;
using System.Collections.Generic;

namespace Momentum.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string DisplayName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string TimeZoneId { get; set; } = null!;

    public string PreferredLanguage { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; }

    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }

    public virtual ICollection<Habit> Habits { get; set; } = new List<Habit>();
}
