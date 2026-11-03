using Momentum.Domain.Entities;

namespace Momentum.Application.Interfaces;

public interface IStreakFreezeRepository
{
    Task<bool> ExistsAsync(
        int habitId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<int> CountInMonthAsync(
        int habitId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task AddAsync(StreakFreeze freeze, CancellationToken cancellationToken = default);
}