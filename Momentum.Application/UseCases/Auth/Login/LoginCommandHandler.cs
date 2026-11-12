using MediatR;
using Momentum.Application.Common;
using Momentum.Application.Interfaces;
using Momentum.Contracts.DTOs.Auth;

namespace Momentum.Application.UseCases.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);

        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
            return Result<AuthResponse>.Failure(Error.Unauthorized());

        // Verify password
        bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValidPassword)
            return Result<AuthResponse>.Failure(Error.Unauthorized());

        var token = _tokenService.GenerateToken(user);

        return Result<AuthResponse>.Success(new AuthResponse(token, user.DisplayName, user.Email));
    }
}