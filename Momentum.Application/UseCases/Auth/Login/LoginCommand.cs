using MediatR;
using Momentum.Application.Common;
using Momentum.Contracts.DTOs.Auth;

namespace Momentum.Application.UseCases.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponse>>;