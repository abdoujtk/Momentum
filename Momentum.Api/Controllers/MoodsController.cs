using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Momentum.Application.UseCases.Moods.GetMoods;

namespace Momentum.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MoodsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMoodsQuery(), ct);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
    }
}