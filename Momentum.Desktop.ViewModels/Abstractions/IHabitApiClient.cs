using Momentum.Contracts;
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
    }
}
