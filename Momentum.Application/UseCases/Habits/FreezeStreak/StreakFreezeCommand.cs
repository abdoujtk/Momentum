using MediatR;
using Momentum.Application.Common;

using Momentum.Contracts.DTOs;

namespace Momentum.Application.UseCases.Habits.FreezeStreak;

public sealed record StreakFreezeCommand(
    int HabitId,
    DateOnly FreezeDate,
    string? Reason
) : IRequest<Result<StreakFreezeResultDto>>;