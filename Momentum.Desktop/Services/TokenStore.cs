using Momentum.Desktop.ViewModels.Abstractions;

namespace Momentum.Desktop.Services;

public class TokenStore : ITokenStore
{
    public string? Token { get; set; }
    public string? DisplayName { get; set; }

    public void Clear()
    {
        Token = null;
        DisplayName = null;
    }
}