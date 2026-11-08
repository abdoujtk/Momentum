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
}