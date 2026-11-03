using Microsoft.EntityFrameworkCore;
using Momentum.Application.Interfaces;
using Momentum.Domain.Entities;
using Momentum.Infrastructure.Persistence;

namespace Momentum.Infrastructure.Repositories;

public class StreakFreezeRepository : IStreakFreezeRepository
{
    private readonly MomentumDbContext _db;

    public StreakFreezeRepository(MomentumDbContext db) => _db = db;

    public async Task<bool> ExistsAsync(
        int habitId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _db.StreakFreezes
            .AsNoTracking()
            .AnyAsync(f => f.HabitId == habitId && f.FreezeDate == date,
                      cancellationToken);
    }

    public async Task<int> CountInMonthAsync(
        int habitId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        return await _db.StreakFreezes
            .AsNoTracking()
            .CountAsync(
                f => f.HabitId == habitId
                     && f.FreezeDate.Year == year
                     && f.FreezeDate.Month == month,
                cancellationToken);
    }

    public async Task AddAsync(StreakFreeze freeze, CancellationToken cancellationToken = default)
        => await _db.StreakFreezes.AddAsync(freeze, cancellationToken);
}