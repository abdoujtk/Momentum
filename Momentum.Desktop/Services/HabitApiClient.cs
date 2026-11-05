using Momentum.Contracts;
using Momentum.Desktop.ViewModels.Abstractions;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Momentum.Desktop.Services;

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
}