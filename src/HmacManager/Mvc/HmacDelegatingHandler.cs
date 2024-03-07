using HmacManager.Components;
using HmacManager.Exceptions;

namespace HmacManager.Mvc;

internal class HmacDelegatingHandler : DelegatingHandler
{
    private readonly IHmacManager _hmacManager;
    public HmacDelegatingHandler(IHmacManager hmacManager) => _hmacManager = hmacManager;

    protected override HttpResponseMessage Send(
        HttpRequestMessage request, 
        CancellationToken cancellationToken
    ) => SendAsync(request, cancellationToken).Result;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var signingResult = await _hmacManager.SignAsync(request);
        if (signingResult.IsSuccess)
        {
            return await base.SendAsync(request, cancellationToken);
        }
        else
        {
            throw new HmacSigningException(signingResult, request);  
        }
    }
}