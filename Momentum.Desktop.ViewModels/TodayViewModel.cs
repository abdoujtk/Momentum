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

    public TodayViewModel(IHabitApiClient api) => _api = api;

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
        if (_loaded) return;   // ← مرة واحدة فقط
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
            _loaded = false;   // ← يسمح بإعادة المحاولة
        }
        catch (TaskCanceledException)
        {
            // المستخدم أغلق الصفحة — تجاهل
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
}