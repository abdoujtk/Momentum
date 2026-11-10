using Momentum.Contracts.DTOs;

namespace Momentum.Desktop.ViewModels.Abstractions;

public interface IHabitApiClient
{
    Task<IReadOnlyList<HabitDto>> GetHabitsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<HabitDto>> GetAllHabitsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MoodDto>> GetMoodsAsync(CancellationToken ct = default);
    Task<HeatmapDto?> GetHeatmapAsync(int days = 90, CancellationToken ct = default);
    Task<IReadOnlyList<HabitStreakDto>> GetStreaksAsync(CancellationToken ct = default);
    Task<HabitDetailsDto?> GetHabitDetailsAsync(int habitId, CancellationToken ct = default);

    // Updated: now accepts full CheckInRequest
    Task<bool> CheckInAsync(int habitId, CheckInRequest request, CancellationToken ct = default);

    Task<bool> CreateHabitAsync(CreateHabitRequest request, CancellationToken ct = default);
    Task<bool> UpdateHabitAsync(int habitId, UpdateHabitRequest request, CancellationToken ct = default);
    Task<bool> DeleteHabitAsync(int habitId, CancellationToken ct = default);
}