using MediatR;
using Momentum.Application.Common;
using Momentum.Application.DTOs;
using Momentum.Application.Interfaces;
using Momentum.Domain.Entities;

namespace Momentum.Application.UseCases.Habits.GetHabitsHeatmap;

public class GetHabitsHeatmapHandler
    : IRequestHandler<GetHabitsHeatmapQuery, Result<HeatmapDto>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly ICheckInRepository _checkInRepository;
    private readonly IStreakFreezeRepository _freezeRepository;
    private readonly ICurrentUserService _currentUser;

    public GetHabitsHeatmapHandler(
        IHabitRepository habitRepository,
        ICheckInRepository checkInRepository,
        IStreakFreezeRepository freezeRepository,
        ICurrentUserService currentUser)
    {
        _habitRepository = habitRepository;
        _checkInRepository = checkInRepository;
        _freezeRepository = freezeRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<HeatmapDto>> Handle(
        GetHabitsHeatmapQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var today = _currentUser.Today;
        var endDate = today.AddDays(-1);           
        var startDate = endDate.AddDays(-(request.Days - 1));

        
        var habits = await _habitRepository
            .GetActiveUserHabitsAsync(userId, cancellationToken);

        if (habits.Count == 0)
            return Result<HeatmapDto>.Success(new HeatmapDto(
                startDate, endDate, 0, 0, 0m, new List<HeatmapDayDto>()));

       
        var checkIns = await _checkInRepository
            .GetByUserHabitsInRangeAsync(userId, startDate, endDate, cancellationToken);

        var freezes = await _freezeRepository
            .GetByUserHabitsInRangeAsync(userId, startDate, endDate, cancellationToken);

        
        var checkInLookup = checkIns.ToDictionary(c => (c.CheckInDate, c.HabitId));
        var freezeLookup = freezes.ToDictionary(f => (f.FreezeDate, f.HabitId));


        var days = Enumerable.Range(0, request.Days)
            .Select(offset => startDate.AddDays(offset))
            .Select(date => BuildDay(date, habits, checkInLookup, freezeLookup))
            .ToList();

        
        var totalScheduled = days.Sum(d => d.ScheduledHabits);
        var totalCompleted = days.Sum(d => d.CompletedHabits);
        var overallPercent = totalScheduled == 0
            ? 0m
            : Math.Round(100m * totalCompleted / totalScheduled, 2);

        return Result<HeatmapDto>.Success(new HeatmapDto(
            startDate,
            endDate,
            totalScheduled,
            totalCompleted,
            overallPercent,
            days));
    }

    private static HeatmapDayDto BuildDay(
        DateOnly date,
        List<Habit> habits,
        Dictionary<(DateOnly, int), Domain.Entities.CheckIn> checkInLookup,
        Dictionary<(DateOnly, int), StreakFreeze> freezeLookup)
    {
        int scheduled = 0, completed = 0, frozen = 0;

        foreach (var habit in habits)
        {
            if (!habit.IsScheduledForDate(date))
                continue;

            scheduled++;

            if (checkInLookup.TryGetValue((date, habit.HabitId), out var ci) && ci.IsCompleted)
                completed++;

            if (freezeLookup.TryGetValue((date, habit.HabitId), out _))
                frozen++;
        }

        var percent = scheduled == 0
            ? 0m
            : Math.Round(100m * completed / scheduled, 2);

        return new HeatmapDayDto(
            Date: date,
            DayOfWeek: (int)date.DayOfWeek,
            ScheduledHabits: scheduled,
            CompletedHabits: completed,
            FrozenHabits: frozen,
            CompletionPercent: percent,
            Level: CalculateLevel(percent));
    }

   
    private static int CalculateLevel(decimal percent) => percent switch
    {
        0m => 0,
        <= 25m => 1,
        <= 50m => 2,
        <= 75m => 3,
        _ => 4
    };


}