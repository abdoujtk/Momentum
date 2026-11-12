
using Microsoft.AspNetCore.Http;
using Momentum.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Momentum.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var idClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var id) ? id : 1; // Fallback for testing, but ideally throw Unauthorized
        }
    }

    public string Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public DateOnly Today
    {
        get
        {
            var timeZoneId = _httpContextAccessor.HttpContext?.User.FindFirst("TimeZoneId")?.Value ?? "Asia/Amman";
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                return DateOnly.FromDateTime(local);
            }
            catch
            {
                return DateOnly.FromDateTime(DateTime.UtcNow);
            }
        }
    }
}