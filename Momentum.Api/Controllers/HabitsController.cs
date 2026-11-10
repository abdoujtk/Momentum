using MediatR;
using Microsoft.AspNetCore.Mvc;

using Momentum.Application.UseCases.Habits.CheckIn;
using Momentum.Application.UseCases.Habits.CreateHabit;
using Momentum.Application.UseCases.Habits.DeleteHabit;
using Momentum.Application.UseCases.Habits.FreezeStreak;
using Momentum.Application.UseCases.Habits.GetHabitDetails;
using Momentum.Application.UseCases.Habits.GetHabits;
using Momentum.Application.UseCases.Habits.GetHabitsHeatmap;
using Momentum.Application.UseCases.Habits.GetHabitStreaks;
using Momentum.Application.UseCases.Habits.UpdateHabit;
using Momentum.Contracts.DTOs;

namespace Momentum.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;



    [HttpGet]
    public async Task<IActionResult> GetToday(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTodayHabitsQuery(),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllHabitsQuery(),
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
        CancellationToken ct)
    {
        var command = new CheckInCommand(
            habitId, request.ActualValue, request.IsCompleted,
            request.MoodId, request.Note);

        var result = await _mediator.Send(command, ct);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
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

    [HttpPut("{habitId:int}")]
    public async Task<IActionResult> Update(
    int habitId,
    [FromBody] UpdateHabitRequest request,
    CancellationToken cancellationToken)
    {
        var command = new UpdateHabitCommand(
            habitId,
            request.Title, request.Description,
            request.TargetValue, request.TargetUnit,
            request.FrequencyType, request.DaysOfWeek,
            request.ColorHex, request.IconName,
            request.CategoryId);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{habitId:int}")]
    public async Task<IActionResult> Delete(
        int habitId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteHabitCommand(habitId),
            cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpGet("{habitId:int}/details")]
    public async Task<IActionResult> GetDetails(
    int habitId, [FromQuery] int historyDays = 30,
    CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetHabitDetailsQuery(habitId, historyDays), ct);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
    }


}