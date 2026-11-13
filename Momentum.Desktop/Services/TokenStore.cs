using Momentum.Desktop.ViewModels.Abstractions;
using System.Threading.Tasks;

namespace Momentum.Desktop.Services;

public class TokenStore : ITokenStore
{
    private readonly ISecureTokenStorage _secureStorage;

    public TokenStore(ISecureTokenStorage secureStorage)
    {
        _secureStorage = secureStorage;
    }

    public string? Token { get; private set; }
    public string? DisplayName { get; private set; }

    public async Task SetTokenAsync(string token, string displayName)
    {
        Token = token;
        DisplayName = displayName;
        await _secureStorage.SaveTokenAsync(token, displayName);
    }

    public async Task LoadTokenAsync()
    {
        var (token, name) = await _secureStorage.GetTokenAsync();
        Token = token;
        DisplayName = name;
    }

    public void Clear()
    {
        Token = null;
        DisplayName = null;
        _secureStorage.ClearToken();
    }
}