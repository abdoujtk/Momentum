using Momentum.Domain.Entities;

namespace Momentum.Application.Interfaces;

public interface IMoodRepository
{
    Task<Mood?> GetByIdAsync(int moodId, CancellationToken cancellationToken = default);
         Task<List<Mood>> GetAllAsync(CancellationToken cancellationToken = default);
}