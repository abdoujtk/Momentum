using Microsoft.EntityFrameworkCore;
using Momentum.Application.Interfaces;
using Momentum.Domain.Entities;
using Momentum.Infrastructure.Persistence;

namespace Momentum.Infrastructure.Repositories;

public class HabitRepository(MomentumDbContext db) : IHabitRepository
{
    private readonly MomentumDbContext _db = db;

    public async Task<List<Habit>> GetActiveUserHabitsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Habits
            .AsNoTracking()
            .Include(h => h.Category)
            .Where(h => h.UserId == userId && !h.IsArchived)
            .OrderBy(h => h.HabitId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Habit?> GetByIdAsync(
        int habitId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Habits
            .AsNoTracking()
            .Include(h => h.Category)
            .FirstOrDefaultAsync(h => h.HabitId == habitId, cancellationToken);
    }
}