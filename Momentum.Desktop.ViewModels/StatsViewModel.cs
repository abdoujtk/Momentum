using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Momentum.Contracts.DTOs;
using Momentum.Desktop.ViewModels.Abstractions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Momentum.Desktop.ViewModels;

public sealed partial class StatsViewModel : ObservableObject
{
    private readonly IHabitApiClient _api;
    private bool _loaded;

    public StatsViewModel(IHabitApiClient api) => _api = api;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private HeatmapDto? _heatmap;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ObservableCollection<HabitStreakDto> Streaks { get; } = new();

    [RelayCommand]
    public async Task LoadAsync(CancellationToken ct = default)
    {
        if (_loaded) return;
        _loaded = true;
        IsLoading = true;
        ErrorMessage = null;

        try
        {
         
            var heatmapTask = _api.GetHeatmapAsync(90, ct);
            var streaksTask = _api.GetStreaksAsync(ct);

            await Task.WhenAll(heatmapTask, streaksTask);

            Heatmap = await heatmapTask;

            Streaks.Clear();
            foreach (var s in await streaksTask)
                Streaks.Add(s);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"تعذر تحميل الإحصائيات: {ex.Message}";
            _loaded = false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnErrorMessageChanged(string? value) => OnPropertyChanged(nameof(HasError));
}