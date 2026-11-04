using MediatR;
using Momentum.Application.Common;

namespace Momentum.Application.UseCases.Habits.DeleteHabit;

public sealed record DeleteHabitCommand(int HabitId) : IRequest<Result>;