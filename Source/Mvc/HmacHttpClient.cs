using HmacManager.Components;
using HmacManager.Exceptions;

namespace HmacManager.Mvc;

public class HmacHttpClient
{
    protected readonly HttpClient _client;
    protected readonly IHmacManagerFactory _hmacManagerFactory;

    public HmacHttpClient(
        HttpClient client,
        IHmacManagerFactory hmacManagerFactory
    )
    {
        _client = client;
        _hmacManagerFactory = hmacManagerFactory;
    }

    public async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        string policy,
        CancellationToken cancellationToken
    )
    {
        var hmacManager = _hmacManagerFactory.Create(policy);
        var signingResult = await hmacManager.SignAsync(request);
        if (signingResult.IsSuccess)
        {
            return await _client.SendAsync(request, cancellationToken);
        }
        else
        {
            throw new HmacSigningException(signingResult, request);  
        }
    }
}