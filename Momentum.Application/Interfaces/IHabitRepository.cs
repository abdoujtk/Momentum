using Momentum.Domain.Entities;

namespace Momentum.Application.Interfaces;

public interface IHabitRepository
{
    
    Task<List<Habit>> GetActiveUserHabitsAsync(int userId, CancellationToken cancellationToken = default);

    Task<Habit?> GetByIdAsync(int habitId, CancellationToken cancellationToken = default);
}