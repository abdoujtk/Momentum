using Momentum.Application.Interfaces;

namespace Momentum.Application.Services;


public class FreezeQuotaService(IStreakFreezeRepository repository) : IFreezeQuotaService
{
    private readonly IStreakFreezeRepository _repository = repository;

    public int MaxFreezesPerMonth => 2;

    public async Task<bool> CanFreezeAsync(
        int habitId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var used = await _repository.CountInMonthAsync(
            habitId, date.Year, date.Month, cancellationToken);

        return used < MaxFreezesPerMonth;
    }
}