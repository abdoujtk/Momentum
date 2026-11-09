using MediatR;
using Momentum.Application.Common;
using Momentum.Contracts.DTOs;

namespace Momentum.Application.UseCases.Habits.GetHabitDetails;

public sealed record GetHabitDetailsQuery(int HabitId, int HistoryDays = 30)
    : IRequest<Result<HabitDetailsDto>>;