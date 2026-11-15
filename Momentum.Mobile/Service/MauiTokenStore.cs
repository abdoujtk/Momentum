using Momentum.Desktop.ViewModels.Abstractions;

namespace Momentum.Mobile.Service;

public class MauiTokenStore : ITokenStore
{
    private const string TokenKey = "auth_token";
    private const string NameKey = "display_name";

    public string? Token { get; private set; }
    public string? DisplayName { get; private set; }

    public async Task SetTokenAsync(string token, string displayName)
    {
        Token = token;
        DisplayName = displayName;
        await SecureStorage.Default.SetAsync(TokenKey, token);
        await SecureStorage.Default.SetAsync(NameKey, displayName);
    }

    public async Task LoadTokenAsync()
    {
        Token = await SecureStorage.Default.GetAsync(TokenKey);
        DisplayName = await SecureStorage.Default.GetAsync(NameKey);
    }

    public void Clear()
    {
        Token = null;
        DisplayName = null;
        SecureStorage.Default.Remove(TokenKey);
        SecureStorage.Default.Remove(NameKey);
    }
}