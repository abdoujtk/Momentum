using MediatR;
using Momentum.Application.Common;
using Momentum.Application.Interfaces;

namespace Momentum.Application.UseCases.Habits.DeleteHabit;

public class DeleteHabitHandler : IRequestHandler<DeleteHabitCommand, Result>
{
    private readonly IHabitRepository _habitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteHabitHandler(
        IHabitRepository habitRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _habitRepository = habitRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        DeleteHabitCommand request,
        CancellationToken cancellationToken)
    {
        var habit = await _habitRepository
            .GetByIdForUpdateAsync(request.HabitId, cancellationToken);

        if (habit is null)
            return Result.Failure(Error.NotFound("Habit"));

        if (habit.UserId != _currentUser.UserId)
            return Result.Failure(Error.Unauthorized());

        if (habit.IsArchived)
            return Result.Failure(Error.Validation("Habit is already archived."));

       
        habit.IsArchived = true;
        habit.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}