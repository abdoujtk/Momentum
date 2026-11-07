using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Momentum.Contracts;
using Momentum.Desktop.ViewModels.Abstractions;

namespace Momentum.Desktop.ViewModels;

public sealed partial class TodayViewModel : ObservableObject
{
    private readonly IHabitApiClient _api;
    private bool _loaded;

    public TodayViewModel(IHabitApiClient api)
    {
        _api = api;
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    partial void OnErrorMessageChanged(string? value)
    => OnPropertyChanged(nameof(HasError));

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isEmpty;

    public ObservableCollection<HabitDto> Habits { get; } = new();

    [RelayCommand]
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
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"تعذر الاتصال بالخادم: {ex.Message}";
            _loaded = false;   
        }
        catch (TaskCanceledException)
        {
            
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync(CancellationToken ct)
    {
        _loaded = false;
        await LoadAsync(ct);
    }

    [RelayCommand(AllowConcurrentExecutions =true)]
    private async Task CheckInAsync(HabitDto? habit)
    {
        if (habit is null || habit.IsCompletedToday) return;

        var index = Habits.IndexOf(habit);
        if (index == -1) return;

        
        var original = habit;
        Habits[index] = habit with { IsCompletedToday = true, TodayActualValue = habit.TargetValue };

        try
        {
            var success = await _api.CheckInAsync(habit.HabitId);
            if (!success)
            {
                
                Habits[index] = original;
            }
        }
        catch (Exception)
        { 
           
            Habits[index] = original;
        }
    }
}