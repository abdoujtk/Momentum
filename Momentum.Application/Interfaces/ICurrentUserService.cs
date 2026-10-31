namespace Momentum.Application.Interfaces;


public interface ICurrentUserService
{
    int UserId { get; }
    string Email { get; }
    DateOnly Today { get; } 
}