using MediatR;
using Momentum.Application.Common;

using Momentum.Application.Interfaces;
using Momentum.Contracts.DTOs;

namespace Momentum.Application.UseCases.Habits.GetHabits;

public sealed record GetTodayHabitsQuery : IRequest<Result<List<HabitDto>>>;


public class GetTodayHabitsHandler : IRequestHandler<GetTodayHabitsQuery, Result<List<HabitDto>>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly ICheckInRepository _checkInRepository;
    private readonly ICurrentUserService _currentUser;

    public GetTodayHabitsHandler(
        IHabitRepository habitRepository,
        ICheckInRepository checkInRepository,
        ICurrentUserService currentUser)
    {
        _habitRepository = habitRepository;
        _checkInRepository = checkInRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<HabitDto>>> Handle(
        GetTodayHabitsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var today = _currentUser.Today;

        var habits = await _habitRepository
            .GetActiveUserHabitsAsync(userId, cancellationToken);

        if (habits.Count == 0)
            return Result<List<HabitDto>>.Success(new List<HabitDto>());

        var ToDayHabits = habits.Where(h=> h.IsScheduledForDate(today) ).ToList();

        var habitIds = ToDayHabits.Select(h => h.HabitId).ToList();
        var todayCheckIns = await _checkInRepository
            .GetByHabitIdsAndDateAsync(habitIds, today, cancellationToken);

        var dtos = ToDayHabits.Select(h =>
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