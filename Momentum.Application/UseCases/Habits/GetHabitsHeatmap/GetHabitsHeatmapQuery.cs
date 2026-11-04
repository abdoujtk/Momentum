using MediatR;
using Momentum.Application.Common;
using Momentum.Application.DTOs;

namespace Momentum.Application.UseCases.Habits.GetHabitsHeatmap;

public sealed record GetHabitsHeatmapQuery(int Days = 90)
    : IRequest<Result<HeatmapDto>>;