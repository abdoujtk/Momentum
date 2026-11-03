using Microsoft.EntityFrameworkCore;
using Momentum.Application.Interfaces;
using Momentum.Domain.Entities;
using Momentum.Infrastructure.Persistence;

namespace Momentum.Infrastructure.Repositories;

public class CategoryRepository(MomentumDbContext db) : ICategoryRepository
{
    

    public async Task<Category?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
        => await db.Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId, cancellationToken);

    public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        => await db.Categories.AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);
}