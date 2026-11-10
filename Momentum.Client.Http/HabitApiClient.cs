using Momentum.Contracts;
using Momentum.Contracts.DTOs;
using Momentum.Desktop.ViewModels.Abstractions;
using System.Net.Http.Json;

namespace Momentum.Client.Http;

public sealed class HabitApiClient(HttpClient http) : IHabitApiClient
{
    private readonly HttpClient _http = http;

    public async Task<IReadOnlyList<HabitDto>> GetHabitsAsync(
        CancellationToken ct = default)
    {
        var response = await _http.GetAsync("api/habits", ct);
        response.EnsureSuccessStatusCode();

        var habits = await response.Content
            .ReadFromJsonAsync<List<HabitDto>>(cancellationToken: ct);

        return habits ?? new List<HabitDto>();
    }

    public async Task<bool> CheckInAsync(int habitId, CancellationToken ct = default)
    {
        var body = new { isCompleted = true };
        var response = await _http.PostAsJsonAsync($"api/habits/{habitId}/checkin", body, ct);
        return response.IsSuccessStatusCode;
    }

    public Task<HeatmapDto?> GetHeatmapAsync(int days = 90, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<HabitStreakDto>> GetStreaksAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<HabitDto>> GetAllHabitsAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("api/habits/all", ct); // افترضنا أن المسار /all
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<HabitDto>>(cancellationToken: ct) ?? new();
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("api/categories", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<CategoryDto>>(cancellationToken: ct) ?? new();
    }

    public async Task<bool> CreateHabitAsync(CreateHabitRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/habits", request, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateHabitAsync(int habitId, UpdateHabitRequest request, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"api/habits/{habitId}", request, ct);
        return response.IsSuccessStatusCode;
    }
    public async Task<bool> DeleteHabitAsync(int habitId, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"api/habits/{habitId}", ct);
        return response.IsSuccessStatusCode;  // 204 No Content
    }

    public async Task<HabitDetailsDto?> GetHabitDetailsAsync(int habitId, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"api/habits/{habitId}/details", ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<HabitDetailsDto>(cancellationToken: ct);
    }

    public async Task<bool> CheckInAsync(
        int habitId, CheckInRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/habits/{habitId}/checkin", request, ct);
        return response.IsSuccessStatusCode;
    }

    
    public async Task<IReadOnlyList<MoodDto>> GetMoodsAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("api/moods", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<MoodDto>>(
            cancellationToken: ct) ?? new();
    }

}