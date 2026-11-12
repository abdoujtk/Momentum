using MediatR;
using Momentum.Application.Common;
using Momentum.Contracts.DTOs.Auth;

namespace Momentum.Application.UseCases.Auth.Register;

public sealed record RegisterCommand(
    string DisplayName,
    string Email,
    string Password,
    string TimeZoneId) : IRequest<Result<AuthResponse>>;