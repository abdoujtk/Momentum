using Momentum.Application.Interfaces;
using Momentum.Infrastructure.Persistence;

namespace Momentum.Infrastructure.Repositories;

public class UnitOfWork(MomentumDbContext db) : IUnitOfWork
{
    private readonly MomentumDbContext _db = db;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}