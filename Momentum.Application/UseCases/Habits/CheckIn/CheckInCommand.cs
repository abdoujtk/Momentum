using MediatR;
using Momentum.Application.Common;
using Momentum.Application.DTOs;

namespace Momentum.Application.UseCases.Habits.CheckIn;

public sealed record CheckInCommand(
    int HabitId,
    decimal? ActualValue,
    bool IsCompleted,
    int? MoodId,
    string? Note
) : IRequest<Result<CheckInDto>>;