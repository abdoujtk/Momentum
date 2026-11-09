using MediatR;
using Momentum.Application.Common;
using Momentum.Application.Interfaces;
using Momentum.Contracts.DTOs;
using Momentum.Domain.Entities;

namespace Momentum.Application.UseCases.Habits.GetHabitDetails;

public class GetHabitDetailsHandler
    : IRequestHandler<GetHabitDetailsQuery, Result<HabitDetailsDto>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly ICheckInRepository _checkInRepository;
    private readonly IStreakFreezeRepository _freezeRepository;
    private readonly ICurrentUserService _currentUser;

    public GetHabitDetailsHandler(
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

    public async Task<Result<HabitDetailsDto>> Handle(
      GetHabitDetailsQuery request, CancellationToken ct)
    {
        var habit = await _habitRepository.GetByIdAsync(request.HabitId, ct);
        if (habit is null) return Result<HabitDetailsDto>.Failure(Error.NotFound("Habit"));
        if (habit.UserId != _currentUser.UserId) return Result<HabitDetailsDto>.Failure(Error.Unauthorized());

        var today = _currentUser.Today;

        var allCheckIns = await _checkInRepository.GetByHabitIdInRangeAsync(
            habit.HabitId, habit.StartDate, today, ct);

        var allFreezes = await _freezeRepository.GetByHabitIdInRangeAsync(
            habit.HabitId, habit.StartDate, today, ct);

      
        var (current, best, lastCompleted, totalCompleted, totalFrozen) =
            CalculateStreaks(habit, allCheckIns, allFreezes, today);

    
        var historyFrom = today.AddDays(-request.HistoryDays);

        var recentCheckIns = allCheckIns
            .Where(c => c.CheckInDate >= historyFrom)
            .OrderByDescending(c => c.CheckInDate)
            .Select(c => new CheckInDto(
                c.CheckInId, c.HabitId, c.CheckInDate,
                c.ActualValue, c.IsCompleted, c.MoodId,
                null, null, c.Note))
            .ToList();

        var recentFreezes = allFreezes
            .Where(f => f.FreezeDate >= historyFrom)
            .OrderByDescending(f => f.FreezeDate)
            .Select(f => new StreakFreezeDto(
                f.FreezeId, f.HabitId, f.FreezeDate, f.Reason))
            .ToList();

   
        var todayCheckIn = allCheckIns.FirstOrDefault(c => c.CheckInDate == today);

        var habitDto = new HabitDto(
            habit.HabitId, habit.Title, habit.Description,
            habit.TargetValue, habit.TargetUnit,
            habit.FrequencyType, habit.DaysOfWeek,
            habit.ColorHex, habit.IconName,
            habit.CategoryId, habit.Category.Name, habit.Category.ColorHex,
            IsCompletedToday: todayCheckIn?.IsCompleted ?? false,
            TodayActualValue: todayCheckIn?.ActualValue);

        return Result<HabitDetailsDto>.Success(new HabitDetailsDto(
            habitDto, current, best, lastCompleted,
            totalCompleted, totalFrozen,
            recentCheckIns, recentFreezes));
    }

    private static (int Current, int Best, DateOnly? LastCompleted, int TotalCompleted, int TotalFrozen)
        CalculateStreaks(Habit habit, List<Domain.Entities.CheckIn> checkIns, List<StreakFreeze> freezes, DateOnly today)
    {
        var checkInMap = checkIns.Where(c => c.IsCompleted).ToDictionary(c => c.CheckInDate);
        var frozenSet = freezes.Select(f => f.FreezeDate).ToHashSet();

        int best = 0, currentRun = 0, totalCompleted = 0, totalFrozen = 0;
        DateOnly? lastCompleted = null;

        var end = habit.EndDate.HasValue && habit.EndDate.Value < today ? habit.EndDate.Value : today;

        for (var d = habit.StartDate; d <= end; d = d.AddDays(1))
        {
            if (!habit.IsScheduledForDate(d)) continue;

            if (checkInMap.ContainsKey(d))
            {
                currentRun++; best = Math.Max(best, currentRun);
                totalCompleted++; lastCompleted = d;
            }
            else if (frozenSet.Contains(d))
            {
                currentRun++; best = Math.Max(best, currentRun);
                totalFrozen++;
            }
            else if (d == today) break;  
            else currentRun = 0;
        }

        return (currentRun, best, lastCompleted, totalCompleted, totalFrozen);
    }
}