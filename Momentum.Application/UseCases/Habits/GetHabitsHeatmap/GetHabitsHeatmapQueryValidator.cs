using FluentValidation;

namespace Momentum.Application.UseCases.Habits.GetHabitsHeatmap;

public class GetHabitsHeatmapQueryValidator : AbstractValidator<GetHabitsHeatmapQuery>
{
    public GetHabitsHeatmapQueryValidator()
    {
        RuleFor(x => x.Days)
            .InclusiveBetween(7, 365)
            .WithMessage("Days must be between 7 and 365.");
    }
}