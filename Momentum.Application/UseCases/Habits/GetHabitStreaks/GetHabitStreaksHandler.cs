using MediatR;
using Momentum.Application.Common;

using Momentum.Application.Interfaces;
using Momentum.Contracts.DTOs;
using Momentum.Domain.Entities;

namespace Momentum.Application.UseCases.Habits.GetHabitStreaks;

public class GetHabitStreaksHandler
    : IRequestHandler<GetHabitStreaksQuery, Result<List<HabitStreakDto>>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly ICheckInRepository _checkInRepository;
    private readonly IStreakFreezeRepository _freezeRepository;
    private readonly ICurrentUserService _currentUser;

    public GetHabitStreaksHandler(
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

    public async Task<Result<List<HabitStreakDto>>> Handle(
        GetHabitStreaksQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var today = _currentUser.Today;

        var habits = await _habitRepository
            .GetActiveUserHabitsAsync(userId, cancellationToken);

        if (habits.Count == 0)
            return Result<List<HabitStreakDto>>.Success(new());

        var earliestStart = habits.Min(h => h.StartDate);

        var checkIns = await _checkInRepository
            .GetByUserHabitsInRangeAsync(userId, earliestStart, today, cancellationToken);

        var freezes = await _freezeRepository
            .GetByUserHabitsInRangeAsync(userId, earliestStart, today, cancellationToken);


        var checkInsByHabit = checkIns
            .GroupBy(c => c.HabitId)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(c => c.CheckInDate));

        var freezesByHabit = freezes
            .GroupBy(f => f.HabitId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.FreezeDate).ToHashSet());

        var results = habits
            .Where(h => h.StartDate <= today)
            .Select(h => ComputeStreak(
                h,
                checkInsByHabit.GetValueOrDefault(h.HabitId) ?? new(),
                freezesByHabit.GetValueOrDefault(h.HabitId) ?? new(),
                today))
            .ToList();

        return Result<List<HabitStreakDto>>.Success(results);
    }

    private static HabitStreakDto ComputeStreak(
        Habit habit,
        Dictionary<DateOnly, Domain.Entities.CheckIn> checkInsByDate,
        HashSet<DateOnly> frozenDates,
        DateOnly today)
    {
        int bestStreak = 0;
        int currentRun = 0;
        int totalCompleted = 0;
        int totalFrozen = 0;
        DateOnly? lastCompleted = null;

        var end = habit.EndDate.HasValue && habit.EndDate.Value < today
            ? habit.EndDate.Value
            : today;

        for (var date = habit.StartDate; date <= end; date = date.AddDays(1))
        {
            if (!habit.IsScheduledForDate(date))
                continue;

            var completed = checkInsByDate.TryGetValue(date, out var ci) && ci.IsCompleted;
            var frozen = frozenDates.Contains(date);

            if (completed)
            {
                currentRun++;
                bestStreak = Math.Max(bestStreak, currentRun);
                totalCompleted++;
                lastCompleted = date;
            }
            else if (frozen)
            {
                currentRun++;
                bestStreak = Math.Max(bestStreak, currentRun);
                totalFrozen++;
            }
            else if (date == today)
            {
                break;
            }
            else
            {
                currentRun = 0;
            }
        }

        return new HabitStreakDto(
            HabitId: habit.HabitId,
            Title: habit.Title,
            CategoryName: habit.Category.Name,
            CurrentStreak: currentRun,
            BestStreak: bestStreak,
            LastCompletedDate: lastCompleted,
            TotalCompletedDays: totalCompleted,
            TotalFrozenDays: totalFrozen);
    }

    
}