using Momentum.Domain.Entities;

namespace Momentum.Application.Interfaces;

public interface ICheckInRepository
{
    
    Task<Dictionary<int, CheckIn>> GetByHabitIdsAndDateAsync(
        IEnumerable<int> habitIds,
        DateOnly date,
        CancellationToken cancellationToken = default);
}