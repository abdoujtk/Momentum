using Microsoft.EntityFrameworkCore;
using Momentum.Application.Interfaces;
using Momentum.Domain.Entities;
using Momentum.Infrastructure.Persistence;

namespace Momentum.Infrastructure.Repositories;

public class MoodRepository(MomentumDbContext db) : IMoodRepository
{
    private readonly MomentumDbContext _db = db;

    public async Task<Mood?> GetByIdAsync(int moodId, CancellationToken cancellationToken = default)
        => await _db.Moods.AsNoTracking()
            .FirstOrDefaultAsync(m => m.MoodId == moodId, cancellationToken);
}