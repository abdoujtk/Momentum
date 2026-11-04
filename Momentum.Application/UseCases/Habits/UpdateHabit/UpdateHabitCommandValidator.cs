using FluentValidation;

namespace Momentum.Application.UseCases.Habits.UpdateHabit;

public class UpdateHabitCommandValidator : AbstractValidator<UpdateHabitCommand>
{
    public UpdateHabitCommandValidator()
    {
        RuleFor(x => x.HabitId).GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150);

        RuleFor(x => x.FrequencyType)
            .NotEmpty()
            .Must(x => x is "Daily" or "SpecificDays")
            .WithMessage("FrequencyType must be 'Daily' or 'SpecificDays'.");

        RuleFor(x => x.DaysOfWeek)
            .NotEmpty()
            .When(x => x.FrequencyType == "SpecificDays")
            .WithMessage("DaysOfWeek is required for SpecificDays.");

        RuleFor(x => x.DaysOfWeek)
            .Matches(@"^[0-6](,[0-6])*$")
            .When(x => !string.IsNullOrEmpty(x.DaysOfWeek))
            .WithMessage("DaysOfWeek must be comma-separated digits 0-6.");

        RuleFor(x => x.DaysOfWeek)
            .Null()
            .When(x => x.FrequencyType == "Daily")
            .WithMessage("DaysOfWeek must be null for Daily frequency.");

        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.TargetValue).GreaterThanOrEqualTo(0).When(x => x.TargetValue.HasValue);
    }
}