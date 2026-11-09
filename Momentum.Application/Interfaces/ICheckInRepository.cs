using Momentum.Domain.Entities;

namespace Momentum.Application.Interfaces;

public interface ICheckInRepository
{
    
    Task<Dictionary<int, CheckIn>> GetByHabitIdsAndDateAsync(
        IEnumerable<int> habitIds,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<CheckIn?> GetByHabitIdAndDateAsync(
    int habitId,
    DateOnly date,
    CancellationToken cancellationToken = default);

    Task AddAsync(CheckIn checkIn, CancellationToken cancellationToken = default);

        Task<List<CheckIn>> GetByUserHabitsInRangeAsync(
    int userId,
    DateOnly startDate,
    DateOnly endDate,
    CancellationToken cancellationToken = default);

    Task<List<CheckIn>> GetByHabitIdInRangeAsync(
    int habitId, DateOnly from, DateOnly to, CancellationToken ct = default);
}