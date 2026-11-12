using Momentum.Domain.Entities;

namespace Momentum.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}