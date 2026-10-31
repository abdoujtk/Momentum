using MediatR;
using Momentum.Application.Common;
using Momentum.Application.DTOs;
using Momentum.Application.Interfaces;

namespace Momentum.Application.UseCases.Habits.GetHabits;

public sealed record GetHabitsQuery : IRequest<Result<List<HabitDto>>>;


public class GetHabitsHandler : IRequestHandler<GetHabitsQuery, Result<List<HabitDto>>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly ICheckInRepository _checkInRepository;
    private readonly ICurrentUserService _currentUser;

    public GetHabitsHandler(
        IHabitRepository habitRepository,
        ICheckInRepository checkInRepository,
        ICurrentUserService currentUser)
    {
        _habitRepository = habitRepository;
        _checkInRepository = checkInRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<HabitDto>>> Handle(
        GetHabitsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var today = _currentUser.Today;

        var habits = await _habitRepository
            .GetActiveUserHabitsAsync(userId, cancellationToken);

        if (habits.Count == 0)
            return Result<List<HabitDto>>.Success(new List<HabitDto>());

        var habitIds = habits.Select(h => h.HabitId).ToList();
        var todayCheckIns = await _checkInRepository
            .GetByHabitIdsAndDateAsync(habitIds, today, cancellationToken);

        var dtos = habits.Select(h =>
        {
            var checkIn = todayCheckIns.GetValueOrDefault(h.HabitId);

            return new HabitDto(
                HabitId: h.HabitId,
                Title: h.Title,
                Description: h.Description,
                TargetValue: h.TargetValue,
                TargetUnit: h.TargetUnit,
                FrequencyType: h.FrequencyType,
                DaysOfWeek: h.DaysOfWeek,
                ColorHex: h.ColorHex,
                IconName: h.IconName,
                CategoryId: h.CategoryId,
                CategoryName: h.Category.Name,
                CategoryColorHex: h.Category.ColorHex,
                IsCompletedToday: checkIn?.IsCompleted ?? false,
                TodayActualValue: checkIn?.ActualValue
            );
        }).ToList();

        return Result<List<HabitDto>>.Success(dtos);
    }
}