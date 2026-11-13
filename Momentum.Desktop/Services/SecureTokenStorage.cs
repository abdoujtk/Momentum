using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Momentum.Desktop.ViewModels.Abstractions;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;

namespace Momentum.Desktop.Services;

public class SecureTokenStorage : ISecureTokenStorage
{
    private readonly string _filePath;
    private readonly string _displayNamePath;

    public SecureTokenStorage()
    {
        var localFolder = ApplicationData.Current.LocalFolder.Path;
        _filePath = Path.Combine(localFolder, "auth.dat");
        _displayNamePath = Path.Combine(localFolder, "user.dat");
    }

    public async Task SaveTokenAsync(string token, string displayName)
    {
        // 1. Encrypt Token (Use descriptor for encryption)
        var provider = new DataProtectionProvider("LOCAL=user");
        var tokenBuffer = CryptographicBuffer.ConvertStringToBinary(token, BinaryStringEncoding.Utf8);
        var encryptedTokenBuffer = await provider.ProtectAsync(tokenBuffer);
        var encryptedTokenBase64 = CryptographicBuffer.EncodeToBase64String(encryptedTokenBuffer);

        // 2. Encrypt DisplayName
        var nameBuffer = CryptographicBuffer.ConvertStringToBinary(displayName, BinaryStringEncoding.Utf8);
        var encryptedNameBuffer = await provider.ProtectAsync(nameBuffer);
        var encryptedNameBase64 = CryptographicBuffer.EncodeToBase64String(encryptedNameBuffer);

        // 3. Save to files
        await File.WriteAllTextAsync(_filePath, encryptedTokenBase64);
        await File.WriteAllTextAsync(_displayNamePath, encryptedNameBase64);
    }

    public async Task<(string? Token, string? DisplayName)> GetTokenAsync()
    {
        if (!File.Exists(_filePath) || !File.Exists(_displayNamePath))
            return (null, null);

        try
        {
            // 2. Decrypt (Use parameterless constructor for decryption)
            var provider = new DataProtectionProvider();

            var encryptedTokenBase64 = await File.ReadAllTextAsync(_filePath);
            var encryptedTokenBuffer = CryptographicBuffer.DecodeFromBase64String(encryptedTokenBase64);
            var tokenBuffer = await provider.UnprotectAsync(encryptedTokenBuffer);
            var token = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, tokenBuffer);

            var encryptedNameBase64 = await File.ReadAllTextAsync(_displayNamePath);
            var encryptedNameBuffer = CryptographicBuffer.DecodeFromBase64String(encryptedNameBase64);
            var nameBuffer = await provider.UnprotectAsync(encryptedNameBuffer);
            var displayName = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, nameBuffer);

            return (token, displayName);
        }
        catch
        {
            ClearToken();
            return (null, null);
        }
    }

    public void ClearToken()
    {
        if (File.Exists(_filePath)) File.Delete(_filePath);
        if (File.Exists(_displayNamePath)) File.Delete(_displayNamePath);
    }
}