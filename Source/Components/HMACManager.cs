using Microsoft.AspNetCore.Http;
using Source.Caching;
using Source.Extensions;

namespace Source.Components;

public interface IHMACManager
{
    Task<bool> VerifyAsync(HttpRequestMessage request);
    Task<RequiredHeaderValues> SignAsync(
        HttpRequestMessage request, 
        MessageContent[]? additionalContent = null
    );
}

public class HMACManager : IHMACManager
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
        if (request.Headers.HasRequiredHeaders(_options.AdditionalContentHeaders, out var headerValues))
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
                    headerValues.Nonce,
                    headerValues.AdditionalContent
                );

                var signature = _provider.ComputeSignature(signingContent);
                return signature == headerValues.Signature;
            }
        }

        return false;
    }

    public async Task<RequiredHeaderValues> SignAsync(HttpRequestMessage request, MessageContent[]? additionalContent = null)
    {
        var headerValues = new RequiredHeaderValues 
            { AdditionalContent = additionalContent };

        headerValues.SigningContent = await _provider.ComputeSigningContentAsync(
            request, 
            headerValues.RequestedOn, 
            headerValues.Nonce,
            additionalContent
        );

        headerValues.Signature = _provider.ComputeSignature(
            headerValues.SigningContent
        );

        request.Headers.AddSignature(headerValues.Signature);
        request.Headers.AddRequestedOn(headerValues.RequestedOn);
        request.Headers.AddNonce(headerValues.Nonce);
        request.Headers.AddAdditionalContent(additionalContent);

        return headerValues;
    }

    public bool HasValidRequestedOn(DateTimeOffset requestedOn) => 
        DateTimeOffset.UtcNow.Subtract(requestedOn) < _options.MaxAge;

    public async Task<bool> HasValidNonceAsync(Guid nonce) => !(await _cache.ContainsAsync(nonce));
}