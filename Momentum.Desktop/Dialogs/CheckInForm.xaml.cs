using Microsoft.UI.Xaml.Controls;
using Momentum.Contracts.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Momentum.Desktop.Dialogs;

// Wrapper for Mood ComboBox display (emoji + name, or "no mood")
internal sealed record MoodChoice(int? MoodId, string DisplayText);

public sealed partial class CheckInForm : UserControl
{
    private readonly HabitDto _habit;

    public CheckInForm(HabitDto habit, IReadOnlyList<MoodDto> moods)
    {
        InitializeComponent();
        _habit = habit;

        // ═══════════════════════════════════════════════════════════
        // Lesson 1: All assignments after InitializeComponent()
        // ═══════════════════════════════════════════════════════════

        // 1) Show target info
        var targetText = habit.TargetValue.HasValue
            ? $"الهدف: {habit.TargetValue} {habit.TargetUnit ?? ""}"
            : "الهدف: إنجاز العادة";
        TargetInfoText.Text = targetText;

        // 2) Pre-fill actual value with target
        if (habit.TargetValue.HasValue)
            ActualValueBox.Text = habit.TargetValue.Value.ToString("0.##");

        // 3) Build mood choices (first option = no mood)
        var choices = new List<MoodChoice> { new(null, "— بدون مزاج —") };
        choices.AddRange(moods.Select(m =>
            new MoodChoice(m.MoodId, $"{m.Emoji} {m.Name}")));

        MoodBox.ItemsSource = choices;
        MoodBox.SelectedIndex = 0;  // Default to "no mood"
    }

    /// <summary>
    /// Builds the CheckInRequest from the form inputs.
    /// Returns validation errors if any.
    /// </summary>
    public (bool IsValid, CheckInRequest? Request, string? ErrorMessage) TryBuildRequest()
    {
        // Parse actual value (optional, but must be valid if provided)
        decimal? actualValue = null;
        if (!string.IsNullOrWhiteSpace(ActualValueBox.Text))
        {
            if (!decimal.TryParse(ActualValueBox.Text, out var parsed))
                return (false, null, "القيمة الفعلية يجب أن تكون رقماً.");
            if (parsed < 0)
                return (false, null, "القيمة الفعلية لا يمكن أن تكون سالبة.");
            actualValue = parsed;
        }

        // Extract mood (nullable)
        var moodChoice = MoodBox.SelectedItem as MoodChoice;
        var moodId = moodChoice?.MoodId;

        // Extract note (nullable)
        var note = string.IsNullOrWhiteSpace(NoteBox.Text) ? null : NoteBox.Text.Trim();

        var request = new CheckInRequest(
            ActualValue: actualValue,
            IsCompleted: true,  // Always true when this dialog is used
            MoodId: moodId,
            Note: note
        );

        return (true, request, null);
    }
}