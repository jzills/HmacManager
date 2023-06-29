using System;
using System.Net.Http;
using System.Threading.Tasks;
using Source.Caching;
using Source.Extensions;

namespace Source.Components;

public class HMACManager
{
    private readonly HMACManagerOptions _options;
    private readonly INonceCache _cache;
    private readonly IHMACProvider _provider;

    public HMACManager(
        HMACManagerOptions options,
        INonceCache cache,
        IHMACProvider provider
    )
    {
        _options = options;
        _cache = cache;
        _provider = provider;
    }

    public async Task<bool> VerifyAsync(HttpRequestMessage request)
    {
        if (request.Headers.HasRequiredHeaders(out var headerValues))
        {
            if (HasValidRequestedOn(headerValues.RequestedOn) && 
                await HasValidNonceAsync(headerValues.Nonce))
            {   
                await _cache.SetAsync(
                    headerValues.Nonce, 
                    headerValues.RequestedOn
                );

                var signingContent = await _provider.ComputeSigningContentAsync(
                    request, 
                    headerValues.RequestedOn, 
                    headerValues.Nonce
                );

                var signature = _provider.ComputeSignature(signingContent);
                return signature == headerValues.Signature;
            }
        }

        return false;
    }

    public async Task<RequiredHeaderValues> SignAsync(HttpRequestMessage request)
    {
        var headerValues = new RequiredHeaderValues();

        headerValues.SigningContent = await _provider.ComputeSigningContentAsync(
            request, 
            headerValues.RequestedOn, 
            headerValues.Nonce
        );

        headerValues.Signature = _provider.ComputeSignature(
            headerValues.SigningContent
        );

        request.Headers.AddSignature(headerValues.Signature);
        request.Headers.AddRequestedOn(headerValues.RequestedOn);
        request.Headers.AddNonce(headerValues.Nonce);

        return headerValues;
    }

    public bool HasValidRequestedOn(DateTimeOffset requestedOn) => 
        DateTimeOffset.UtcNow.Subtract(requestedOn) < _options.MaxAge;

    public async Task<bool> HasValidNonceAsync(Guid nonce) => !(await _cache.ContainsAsync(nonce));
}