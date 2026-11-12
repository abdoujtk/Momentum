using MediatR;
using Microsoft.AspNetCore.Mvc;
using Momentum.Application.UseCases.Auth.Login;
using Momentum.Application.UseCases.Auth.Register;
using Momentum.Contracts.DTOs.Auth;

namespace Momentum.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var command = new RegisterCommand(request.DisplayName, request.Email, request.Password, request.TimeZoneId);
        var result = await mediator.Send(command, ct);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await mediator.Send(command, ct);
        return result.IsFailure ? Unauthorized(result.Error) : Ok(result.Value);
    }
}