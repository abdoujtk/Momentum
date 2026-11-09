using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Momentum.Contracts.DTOs;
using Momentum.Desktop.ViewModels.Abstractions;
using System.Collections.ObjectModel;

namespace Momentum.Desktop.ViewModels;

public sealed partial class HabitDetailsViewModel : ObservableObject
{
    private readonly IHabitApiClient _api;
    private readonly INavigationService _navigation;
    private int _currentHabitId;
    private bool _loaded;

    public HabitDetailsViewModel(IHabitApiClient api, INavigationService navigation)
    {
        _api = api;
        _navigation = navigation;
    }

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private HabitDetailsDto? _details;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool HasDetails => Details is not null;

    public ObservableCollection<CheckInDto> RecentCheckIns { get; } = new();
    public ObservableCollection<StreakFreezeDto> RecentFreezes { get; } = new();

    public async Task LoadAsync(int habitId, CancellationToken ct = default)
    {
        // Skip reload if same habit is already loaded
        if (_loaded && _currentHabitId == habitId) return;

        _currentHabitId = habitId;
        _loaded = false;
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var details = await _api.GetHabitDetailsAsync(habitId, ct);
            if (details is null)
            {
                ErrorMessage = "Could not load habit details.";
                return;
            }

            Details = details;

            RecentCheckIns.Clear();
            foreach (var ci in details.RecentCheckIns) RecentCheckIns.Add(ci);

            RecentFreezes.Clear();
            foreach (var f in details.RecentFreezes) RecentFreezes.Add(f);

            _loaded = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load details: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void GoBack() => _navigation.GoBack();

    partial void OnErrorMessageChanged(string? value) => OnPropertyChanged(nameof(HasError));
    partial void OnDetailsChanged(HabitDetailsDto? value) => OnPropertyChanged(nameof(HasDetails));
}