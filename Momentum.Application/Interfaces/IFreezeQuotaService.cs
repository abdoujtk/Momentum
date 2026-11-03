namespace Momentum.Application.Interfaces;

public interface IFreezeQuotaService
{
    int MaxFreezesPerMonth { get; }

    Task<bool> CanFreezeAsync(
        int habitId,
        DateOnly date,
        CancellationToken cancellationToken = default);
}