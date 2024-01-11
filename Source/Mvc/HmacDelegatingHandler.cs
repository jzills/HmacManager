using HmacManager.Components;

namespace HmacManager.Mvc;

public class HmacDelegatingHandler : DelegatingHandler
{
    private readonly IHmacManager _hmacManager;

    public HmacDelegatingHandler(IHmacManager hmacManager) => 
        _hmacManager = hmacManager;

    protected override HttpResponseMessage Send(
        HttpRequestMessage request, 
        CancellationToken cancellationToken
    )
    {
        _hmacManager.SignAsync(request).RunSynchronously();

        return base.Send(request, cancellationToken);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken
    )
    {
        await _hmacManager.SignAsync(request);
        
        return await base.SendAsync(request, cancellationToken);
    }
}