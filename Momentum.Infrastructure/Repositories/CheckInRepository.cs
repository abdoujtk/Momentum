using Microsoft.EntityFrameworkCore;
using Momentum.Application.Interfaces;
using Momentum.Domain.Entities;
using Momentum.Infrastructure.Persistence;

namespace Momentum.Infrastructure.Repositories;

public class CheckInRepository(MomentumDbContext db) : ICheckInRepository
{
    private readonly MomentumDbContext _db = db;

    public async Task<Dictionary<int, CheckIn>> GetByHabitIdsAndDateAsync(
        IEnumerable<int> habitIds,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var ids = habitIds.ToList();
        if (ids.Count == 0)
            return new Dictionary<int, CheckIn>();

        return await _db.CheckIns
            .AsNoTracking() 
            .Where(c => ids.Contains(c.HabitId) && c.CheckInDate == date)
            .ToDictionaryAsync(c => c.HabitId, cancellationToken);
    }
}