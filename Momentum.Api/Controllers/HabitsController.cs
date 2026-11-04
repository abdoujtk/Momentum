using MediatR;
using Microsoft.AspNetCore.Mvc;
using Momentum.Application.DTOs;
using Momentum.Application.UseCases.Habits.CheckIn;
using Momentum.Application.UseCases.Habits.CreateHabit;
using Momentum.Application.UseCases.Habits.FreezeStreak;
using Momentum.Application.UseCases.Habits.GetHabits;
using Momentum.Application.UseCases.Habits.GetHabitsHeatmap;
using Momentum.Application.UseCases.Habits.GetHabitStreaks;

namespace Momentum.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetHabitsQuery(),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
    [FromBody] CreateHabitRequest request,
    CancellationToken cancellationToken)
    {
        var command = new CreateHabitCommand(
            request.Title, request.Description,
            request.TargetValue, request.TargetUnit,
            request.FrequencyType, request.DaysOfWeek,
            request.ColorHex, request.IconName,
            request.CategoryId);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Created($"/api/habits/{result.Value!.HabitId}", result.Value);
    }



[HttpPost("{habitId:int}/checkin")]
public async Task<IActionResult> CheckIn(
    int habitId,
    [FromBody] CheckInRequest request,
    CancellationToken cancellationToken)
{
    var command = new CheckInCommand(
        habitId,
        request.ActualValue,
        request.IsCompleted,
        request.MoodId,
        request.Note);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure)
        return BadRequest(result.Error);

    return Ok(result.Value);
}

    [HttpPost("{habitId:int}/freeze")]
    public async Task<IActionResult> Freeze(
    int habitId,
    [FromBody] StreakFreezeRequest request,
    CancellationToken cancellationToken)
    {
        var command = new StreakFreezeCommand(
            habitId,
            request.FreezeDate,
            request.Reason);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("heatmap")]
    public async Task<IActionResult> GetHeatmap(
        [FromQuery] int days = 90,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetHabitsHeatmapQuery(days),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("streaks")]
    public async Task<IActionResult> GetStreaks(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetHabitStreaksQuery(),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

}