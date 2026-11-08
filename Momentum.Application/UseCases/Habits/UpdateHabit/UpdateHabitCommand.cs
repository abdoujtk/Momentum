using MediatR;
using Momentum.Application.Common;

using Momentum.Contracts.DTOs;

namespace Momentum.Application.UseCases.Habits.UpdateHabit;

public sealed record UpdateHabitCommand(
    int HabitId,
    string Title,
    string? Description,
    decimal? TargetValue,
    string? TargetUnit,
    string FrequencyType,
    string? DaysOfWeek,
    string? ColorHex,
    string? IconName,
    int CategoryId
) : IRequest<Result<HabitDto>>;