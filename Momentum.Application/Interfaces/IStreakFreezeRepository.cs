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

    Task<List<StreakFreeze>> GetByUserHabitsInRangeAsync(
    int userId,
    DateOnly startDate,
    DateOnly endDate,
    CancellationToken cancellationToken = default);

    Task<List<StreakFreeze>> GetByHabitIdInRangeAsync(
    int habitId, DateOnly from, DateOnly to, CancellationToken ct = default);
}