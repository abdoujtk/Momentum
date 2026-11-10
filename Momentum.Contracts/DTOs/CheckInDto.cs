namespace Momentum.Contracts.DTOs;

public sealed record CheckInDto(
    int CheckInId,
    int HabitId,
    DateOnly CheckInDate,
    decimal? ActualValue,
    decimal? TargetValue,
    string? TargetUnit,
    bool IsCompleted,
    int? MoodId,
    string? MoodName,
    string? MoodEmoji,
    string? Note
)
{
    /// <summary>True if a mood is recorded (used for conditional visibility in UI).</summary>
    public bool HasMood => !string.IsNullOrEmpty(MoodName);

    /// <summary>Formatted mood for display: "😄 ممتاز" or empty.</summary>
    public string DisplayMood => HasMood
        ? $"{MoodEmoji} {MoodName}".Trim()
        : string.Empty;

    /// <summary>
    /// Formatted value with unit and target: "19 / 20 صفحة" or "30 دقيقة".
    /// Falls back to "✓" if completed without a numeric value.
    /// </summary>
    public string DisplayValue
    {
        get
        {
            if (!ActualValue.HasValue)
                return IsCompleted ? "✓" : string.Empty;

            var actual = ActualValue.Value.ToString("0.##");
            var unit = TargetUnit ?? string.Empty;

            return TargetValue.HasValue
                ? $"{actual} / {TargetValue.Value:0.##} {unit}".Trim()
                : $"{actual} {unit}".Trim();
        }
    }
}