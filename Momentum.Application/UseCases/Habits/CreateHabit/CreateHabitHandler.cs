using MediatR;
using Momentum.Application.Common;

using Momentum.Application.Interfaces;
using Momentum.Contracts.DTOs;
using Momentum.Domain.Entities;

namespace Momentum.Application.UseCases.Habits.CreateHabit;

public class CreateHabitHandler : IRequestHandler<CreateHabitCommand, Result<HabitDto>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateHabitHandler(
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
        CreateHabitCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return Result<HabitDto>.Failure(Error.NotFound("Category"));

        var habit = new Habit
        {
            UserId = _currentUser.UserId,
            CategoryId = request.CategoryId,
            Title = request.Title,
            Description = request.Description,
            TargetValue = request.TargetValue,
            TargetUnit = request.TargetUnit,
            FrequencyType = request.FrequencyType,
            DaysOfWeek = request.DaysOfWeek,
            ColorHex = request.ColorHex,
            IconName = request.IconName,
            StartDate = _currentUser.Today,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _habitRepository.AddAsync(habit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new HabitDto(
            habit.HabitId, habit.Title, habit.Description,
            habit.TargetValue, habit.TargetUnit,
            habit.FrequencyType, habit.DaysOfWeek,
            habit.ColorHex, habit.IconName,
            habit.CategoryId, category.Name, category.ColorHex,
            IsCompletedToday: false,
            TodayActualValue: null);

        return Result<HabitDto>.Success(dto);
    }
}