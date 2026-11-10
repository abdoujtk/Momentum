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

    public async Task<CheckIn?> GetByHabitIdAndDateAsync(
    int habitId,
    DateOnly date,
    CancellationToken cancellationToken = default)
    {
        return await _db.CheckIns
            .FirstOrDefaultAsync(
                c => c.HabitId == habitId && c.CheckInDate == date,
                cancellationToken);

    }

    public async Task AddAsync(CheckIn checkIn, CancellationToken cancellationToken = default)
        => await _db.CheckIns.AddAsync(checkIn, cancellationToken);

    public async Task<List<CheckIn>> GetByUserHabitsInRangeAsync(
    int userId,
    DateOnly startDate,
    DateOnly endDate,
    CancellationToken cancellationToken = default)
    {
        return await _db.CheckIns
            .AsNoTracking()
            .Where(c => c.Habit.UserId == userId
                     && c.CheckInDate >= startDate
                     && c.CheckInDate <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CheckIn>> GetByHabitIdInRangeAsync(
    int habitId, DateOnly from, DateOnly to, CancellationToken ct = default)
    => await _db.CheckIns
        .AsNoTracking()
        .Include(c => c.Mood)   // ← NEW: load mood navigation
        .Where(c => c.HabitId == habitId && c.CheckInDate >= from && c.CheckInDate <= to)
        .OrderByDescending(c => c.CheckInDate)
        .ToListAsync(ct);
}