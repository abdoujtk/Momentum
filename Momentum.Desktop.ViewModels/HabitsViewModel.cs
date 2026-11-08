using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Momentum.Contracts.DTOs;
using Momentum.Desktop.ViewModels.Abstractions;
using System.Collections.ObjectModel;

namespace Momentum.Desktop.ViewModels;

public sealed partial class HabitsViewModel : ObservableObject
{
    private readonly IHabitApiClient _api;
    private bool _loaded;

    public HabitsViewModel(IHabitApiClient api) => _api = api;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _errorMessage;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ObservableCollection<HabitDto> Habits { get; } = new();
    public ObservableCollection<CategoryDto> Categories { get; } = new();

    [RelayCommand]
    public async Task LoadAsync(CancellationToken ct = default)
    {
        if (_loaded) return;
        _loaded = true;
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var habitsTask = _api.GetAllHabitsAsync(ct);
            var categoriesTask = _api.GetCategoriesAsync(ct);

            await Task.WhenAll(habitsTask, categoriesTask);

            Habits.Clear();
            foreach (var h in await habitsTask) Habits.Add(h);

            Categories.Clear();
            foreach (var c in await categoriesTask) Categories.Add(c);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"تعذر تحميل البيانات: {ex.Message}";
            _loaded = false; // السماح بإعادة المحاولة
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnErrorMessageChanged(string? value) => OnPropertyChanged(nameof(HasError));


    public async Task<bool> CreateHabitAsync(CreateHabitRequest request)
    {
        try
        {
            var success = await _api.CreateHabitAsync(request);
            if (success)
            {
                _loaded = false;
                await LoadAsync();
            }
            return success;
        }
        catch
        {
            return false;
        }
    }
}