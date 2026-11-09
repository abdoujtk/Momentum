using MediatR;
using Momentum.Application.Common;

using Momentum.Application.Interfaces;
using Momentum.Contracts.DTOs;
using Momentum.Domain.Entities;

namespace Momentum.Application.UseCases.Habits.FreezeStreak;

public class StreakFreezeHandler
    : IRequestHandler<StreakFreezeCommand, Result<StreakFreezeResultDto>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly ICheckInRepository _checkInRepository;
    private readonly IStreakFreezeRepository _freezeRepository;
    private readonly IFreezeQuotaService _quotaService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public StreakFreezeHandler(
        IHabitRepository habitRepository,
        ICheckInRepository checkInRepository,
        IStreakFreezeRepository freezeRepository,
        IFreezeQuotaService quotaService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _habitRepository = habitRepository;
        _checkInRepository = checkInRepository;
        _freezeRepository = freezeRepository;
        _quotaService = quotaService;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<StreakFreezeResultDto>> Handle(
        StreakFreezeCommand request,
        CancellationToken cancellationToken)
    {

        var habit = await _habitRepository.GetByIdAsync(request.HabitId, cancellationToken);
        if (habit is null)
            return Result<StreakFreezeResultDto>.Failure(Error.NotFound("Habit"));


        if (habit.UserId != _currentUser.UserId)
            return Result<StreakFreezeResultDto>.Failure(Error.Unauthorized());


        if (habit.IsArchived)
            return Result<StreakFreezeResultDto>.Failure(
                Error.Validation("Cannot freeze an archived habit."));


        if (request.FreezeDate < habit.StartDate)
            return Result<StreakFreezeResultDto>.Failure(
                Error.Validation("Freeze date cannot be before habit start date."));


        if (!habit.IsScheduledForDate(request.FreezeDate))
            return Result<StreakFreezeResultDto>.Failure(
                Error.Validation("This habit was not scheduled on the requested date."));


        var existingCheckIn = await _checkInRepository
            .GetByHabitIdAndDateAsync(habit.HabitId, request.FreezeDate, cancellationToken);
        if (existingCheckIn is not null)
            return Result<StreakFreezeResultDto>.Failure(
                Error.Validation("Cannot freeze a day that already has a check-in."));


        var alreadyFrozen = await _freezeRepository
            .ExistsAsync(habit.HabitId, request.FreezeDate, cancellationToken);
        if (alreadyFrozen)
            return Result<StreakFreezeResultDto>.Failure(
                Error.Validation("This day is already frozen."));


        var canFreeze = await _quotaService
            .CanFreezeAsync(habit.HabitId, request.FreezeDate, cancellationToken);
        if (!canFreeze)
            return Result<StreakFreezeResultDto>.Failure(
                Error.Validation(
                    $"Monthly freeze quota reached ({_quotaService.MaxFreezesPerMonth} per habit)."));


        var freeze = new StreakFreeze
        {
            HabitId = habit.HabitId,
            FreezeDate = request.FreezeDate,
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow
        };

        await _freezeRepository.AddAsync(freeze, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 10) حساب المتبقي بعد الإضافة
        var used = await _freezeRepository.CountInMonthAsync(
            habit.HabitId, request.FreezeDate.Year, request.FreezeDate.Month,
            cancellationToken);
        var remaining = Math.Max(0, _quotaService.MaxFreezesPerMonth - used);



        return Result<StreakFreezeResultDto>.Success(new StreakFreezeResultDto(
            FreezeId: freeze.FreezeId,
            HabitId: freeze.HabitId,
            FreezeDate: freeze.FreezeDate,
            Reason: freeze.Reason,
            RemainingFreezesThisMonth: remaining
           ));
    }


}