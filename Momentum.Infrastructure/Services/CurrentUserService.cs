using Momentum.Application.Interfaces;

namespace Momentum.Infrastructure.Services;


public class CurrentUserService : ICurrentUserService
{
    public int UserId => 1;
    public string Email => "sara@momentum.app";

    public DateOnly Today
    {
        get
        {
            
            var ammanTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Amman");
            var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ammanTz);
            return DateOnly.FromDateTime(local);
        }
    }
}