using FluentValidation;
using Momentum.Application.Interfaces;

namespace Momentum.Application.UseCases.Habits.FreezeStreak;

public class StreakFreezeCommandValidator : AbstractValidator<StreakFreezeCommand>
{
    public StreakFreezeCommandValidator(ICurrentUserService currentUser)
    {
        RuleFor(x => x.HabitId).GreaterThan(0);

        
        RuleFor(x => x.FreezeDate)
            .LessThan(currentUser.Today)
            .WithMessage("Freeze date must be strictly in the past.");

        RuleFor(x => x.Reason)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}