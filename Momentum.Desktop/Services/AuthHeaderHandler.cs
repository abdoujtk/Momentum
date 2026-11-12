using Momentum.Desktop.ViewModels.Abstractions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Momentum.Desktop.Services;

public class AuthHeaderHandler(ITokenStore tokenStore) : DelegatingHandler
{
    private readonly ITokenStore _tokenStore = tokenStore;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // If we have a token, attach it to the request
        if (!string.IsNullOrEmpty(_tokenStore.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStore.Token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}