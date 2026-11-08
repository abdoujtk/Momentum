using Momentum.Contracts;
using Momentum.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momentum.Desktop.ViewModels.Abstractions
{

    public interface IHabitApiClient
    {
        Task<IReadOnlyList<HabitDto>> GetHabitsAsync(CancellationToken ct = default);
        Task<bool> CheckInAsync(int habitId, CancellationToken ct = default);
    }
}
