using MediatR;
using Momentum.Application.Common;
using Momentum.Application.DTOs;
using Momentum.Application.Interfaces;
using Momentum.Domain.Entities;
namespace Momentum.Application.UseCases.Habits.CheckIn;

public class CheckInHandler : IRequestHandler<CheckInCommand, Result<CheckInDto>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly ICheckInRepository _checkInRepository;
    private readonly IMoodRepository _moodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CheckInHandler(
        IHabitRepository habitRepository,
        ICheckInRepository checkInRepository,
        IMoodRepository moodRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _habitRepository = habitRepository;
        _checkInRepository = checkInRepository;
        _moodRepository = moodRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<CheckInDto>> Handle(
        CheckInCommand request,
        CancellationToken cancellationToken)
    {
        
        var habit = await _habitRepository.GetByIdAsync(request.HabitId, cancellationToken);
        if (habit is null)
            return Result<CheckInDto>.Failure(Error.NotFound("Habit"));

     
        if (habit.UserId != _currentUser.UserId)
            return Result<CheckInDto>.Failure(Error.Unauthorized());

        
        if (habit.IsArchived)
            return Result<CheckInDto>.Failure(
                Error.Validation("Cannot check in an archived habit."));

        
        var today = _currentUser.Today;
        if (!habit.IsScheduledForDate( today))
            return Result<CheckInDto>.Failure(
                Error.Validation("This habit is not scheduled for today."));

       
        Mood? mood = null;
        if (request.MoodId.HasValue)
        {
            mood = await _moodRepository.GetByIdAsync(request.MoodId.Value, cancellationToken);
            if (mood is null)
                return Result<CheckInDto>.Failure(Error.NotFound("Mood"));
        }

        
        var existing = await _checkInRepository
            .GetByHabitIdAndDateAsync(habit.HabitId, today, cancellationToken);

        Domain.Entities.CheckIn checkIn;
        if (existing is null)
        {
            checkIn = new Domain.Entities.CheckIn
            {
                HabitId = habit.HabitId,
                CheckInDate = today,
                ActualValue = request.ActualValue,
                IsCompleted = request.IsCompleted,
                MoodId = request.MoodId,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow
            };
            await _checkInRepository.AddAsync(checkIn, cancellationToken);
        }
        else
        {
            
            existing.ActualValue = request.ActualValue;
            existing.IsCompleted = request.IsCompleted;
            existing.MoodId = request.MoodId;
            existing.Note = request.Note;
            checkIn = existing;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CheckInDto>.Success(new CheckInDto(
            CheckInId: checkIn.CheckInId,
            HabitId: checkIn.HabitId,
            CheckInDate: checkIn.CheckInDate,
            ActualValue: checkIn.ActualValue,
            IsCompleted: checkIn.IsCompleted,
            MoodId: checkIn.MoodId,
            MoodName: mood?.Name,
            MoodEmoji: mood?.Emoji,
            Note: checkIn.Note));
    }

  
    private static bool IsScheduledForDate(Habit habit, DateOnly date)
    {
        if (habit.FrequencyType == "Daily")
            return true;

        if (habit.FrequencyType == "SpecificDays" && !string.IsNullOrEmpty(habit.DaysOfWeek))
        {
            
            var dow = (int)date.DayOfWeek;
            var days = habit.DaysOfWeek.Split(',').Select(int.Parse).ToHashSet();
            return days.Contains(dow);
        }

        return false;
    }
}