namespace Momentum.Desktop.ViewModels.Abstractions;

public interface ITokenStore
{
    string? Token { get; set; }
    string? DisplayName { get; set; }
    void Clear();
}