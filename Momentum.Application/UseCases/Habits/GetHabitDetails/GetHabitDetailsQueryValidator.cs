using FluentValidation;

namespace Momentum.Application.UseCases.Habits.GetHabitDetails;

public class GetHabitDetailsQueryValidator : AbstractValidator<GetHabitDetailsQuery>
{
    public GetHabitDetailsQueryValidator()
    {
        RuleFor(x => x.HabitId).GreaterThan(0);
        RuleFor(x => x.HistoryDays).InclusiveBetween(7, 365);
    }
}