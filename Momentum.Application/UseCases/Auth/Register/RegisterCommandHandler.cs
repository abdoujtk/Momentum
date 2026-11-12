using MediatR;
using Momentum.Application.Common;
using Momentum.Application.Interfaces;
using Momentum.Contracts.DTOs.Auth;
using Momentum.Domain.Entities;

namespace Momentum.Application.UseCases.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await _userRepository.ExistsAsync(request.Email, ct))
            return Result<AuthResponse>.Failure(Error.Validation("Email is already registered."));

        // Hash password using BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            DisplayName = request.DisplayName,
            Email = request.Email,
            PasswordHash = passwordHash,
            TimeZoneId = request.TimeZoneId,
            PreferredLanguage = "ar",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var token = _tokenService.GenerateToken(user);

        return Result<AuthResponse>.Success(new AuthResponse(token, user.DisplayName, user.Email));
    }
}