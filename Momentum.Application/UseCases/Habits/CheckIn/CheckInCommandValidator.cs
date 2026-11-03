using FluentValidation;

namespace Momentum.Application.UseCases.Habits.CheckIn;

public class CheckInCommandValidator : AbstractValidator<CheckInCommand>
{
    public CheckInCommandValidator()
    {
        RuleFor(x => x.HabitId).GreaterThan(0);

        RuleFor(x => x.ActualValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ActualValue.HasValue);

        RuleFor(x => x.MoodId)
            .InclusiveBetween(1, 5)
            .When(x => x.MoodId.HasValue);

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Note));
    }
}