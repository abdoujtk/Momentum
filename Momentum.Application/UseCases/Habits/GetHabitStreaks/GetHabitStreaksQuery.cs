using MediatR;
using Momentum.Application.Common;

using Momentum.Contracts.DTOs;

namespace Momentum.Application.UseCases.Habits.GetHabitStreaks;

public sealed record GetHabitStreaksQuery
    : IRequest<Result<List<HabitStreakDto>>>;