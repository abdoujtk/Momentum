using CommunityToolkit.Mvvm.ComponentModel;
using Momentum.Contracts.DTOs;
using Momentum.Desktop.ViewModels.Abstractions;
using System.Collections.ObjectModel;

namespace Momentum.Desktop.ViewModels;

public sealed partial class TodayViewModel : ObservableObject
{
    private readonly IHabitApiClient _api;
    private bool _loaded;

    public TodayViewModel(IHabitApiClient api) => _api = api;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isEmpty;

    public ObservableCollection<HabitDto> Habits { get; } = new();
    public ObservableCollection<MoodDto> Moods { get; } = new();

    public async Task LoadAsync(CancellationToken ct = default)
    {
        if (_loaded) return;
        _loaded = true;

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var habits = await _api.GetHabitsAsync(ct);
            Habits.Clear();
            foreach (var h in habits) Habits.Add(h);
            IsEmpty = Habits.Count == 0;

            // Load moods once (needed for CheckInForm)
            if (Moods.Count == 0)
            {
                var moods = await _api.GetMoodsAsync(ct);
                foreach (var m in moods) Moods.Add(m);
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"تعذر الاتصال بالخادم: {ex.Message}";
            _loaded = false;
        }
        catch (TaskCanceledException)
        {
            // User navigated away
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Performs the check-in with optimistic UI.
    /// Returns true on success, false on failure (with rollback).
    /// </summary>
    public async Task<bool> CheckInAsync(HabitDto habit, CheckInRequest request)
    {
        var index = Habits.IndexOf(habit);
        if (index == -1) return false;

        var original = habit;

        // Optimistic UI update
        Habits[index] = habit with
        {
            IsCompletedToday = request.IsCompleted,
            TodayActualValue = request.ActualValue ?? habit.TargetValue
        };

        try
        {
            var success = await _api.CheckInAsync(habit.HabitId, request);
            if (!success)
            {
                Habits[index] = original;
                return false;
            }
            return true;
        }
        catch
        {
            Habits[index] = original;
            return false;
        }
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    partial void OnErrorMessageChanged(string? value) => OnPropertyChanged(nameof(HasError));
}