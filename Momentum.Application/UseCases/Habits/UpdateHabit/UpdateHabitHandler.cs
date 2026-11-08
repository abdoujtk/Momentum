using MediatR;
using Momentum.Application.Common;

using Momentum.Application.Interfaces;
using Momentum.Contracts.DTOs;

namespace Momentum.Application.UseCases.Habits.UpdateHabit;

public class UpdateHabitHandler
    : IRequestHandler<UpdateHabitCommand, Result<HabitDto>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateHabitHandler(
        IHabitRepository habitRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _habitRepository = habitRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<HabitDto>> Handle(
        UpdateHabitCommand request,
        CancellationToken cancellationToken)
    {
        var habit = await _habitRepository
            .GetByIdForUpdateAsync(request.HabitId, cancellationToken);

        if (habit is null)
            return Result<HabitDto>.Failure(Error.NotFound("Habit"));

        if (habit.UserId != _currentUser.UserId)
            return Result<HabitDto>.Failure(Error.Unauthorized());

        if (habit.IsArchived)
            return Result<HabitDto>.Failure(
                Error.Validation("Cannot update an archived habit."));

       
        if (habit.CategoryId != request.CategoryId)
        {
            var category = await _categoryRepository
                .GetByIdAsync(request.CategoryId, cancellationToken);
            if (category is null)
                return Result<HabitDto>.Failure(Error.NotFound("Category"));
        }

     
        habit.Title = request.Title;
        habit.Description = request.Description;
        habit.TargetValue = request.TargetValue;
        habit.TargetUnit = request.TargetUnit;
        habit.FrequencyType = request.FrequencyType;
        habit.DaysOfWeek = request.DaysOfWeek;
        habit.ColorHex = request.ColorHex;
        habit.IconName = request.IconName;
        habit.CategoryId = request.CategoryId;
        habit.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);


        var dto = new HabitDto(
            habit.HabitId, habit.Title, habit.Description,
            habit.TargetValue, habit.TargetUnit,
            habit.FrequencyType, habit.DaysOfWeek,
            habit.ColorHex, habit.IconName,
            habit.CategoryId, habit.Category.Name, habit.Category.ColorHex,
            IsCompletedToday: false,
            TodayActualValue: null);

        return Result<HabitDto>.Success(dto);
    }
}