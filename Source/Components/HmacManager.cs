using HmacManagement.Caching;
using HmacManagement.Caching.Extensions;
using HmacManagement.Exceptions;
using HmacManagement.Extensions;

namespace HmacManagement.Components;

public class HmacManager : IHmacManager
{
    private readonly HmacManagerOptions _options;
    private readonly INonceCache _cache;
    private readonly IHmacProvider _provider;

    public HmacManager(
        HmacManagerOptions options,
        INonceCache cache,
        IHmacProvider provider
    )
    {
        _options = options;
        _cache = cache;
        _provider = provider;
    }

    public async Task<HmacResult> VerifyAsync(HttpRequestMessage request)
    {
        if (request.Headers.TryParseHmac(_options.HeaderScheme, _options.MaxAge, out var hmac))
        {
            if (await _cache.HasValidNonceAsync(hmac.Nonce))
            {   
                await _cache.SetAsync(
                    hmac.Nonce, 
                    hmac.RequestedOn
                );

                hmac.SigningContent = await _provider.ComputeSigningContentAsync(
                    request, 
                    hmac.RequestedOn, 
                    hmac.Nonce,
                    hmac.HeaderValues
                );

                var signature = _provider.ComputeSignature(hmac.SigningContent);
                return new HmacResult
                {
                    Hmac = hmac,
                    IsSuccess = signature == hmac.Signature
                };
            }
        }

        return new HmacResult();
    }

    public async Task<HmacResult> SignAsync(HttpRequestMessage request)
    {
        if (request.Headers.TryParseHeaders(_options.HeaderScheme, out var headerValues))
        {   
            var hmac = new Hmac { HeaderValues = headerValues.ToArray() };

            hmac.SigningContent = await _provider.ComputeSigningContentAsync(
                request, 
                hmac.RequestedOn, 
                hmac.Nonce,
                hmac.HeaderValues
            );

            hmac.Signature = _provider.ComputeSignature(
                hmac.SigningContent
            );

            // Add required headers to the request
            request.Headers.AddNonce(hmac.Nonce);
            request.Headers.AddRequestedOn(hmac.RequestedOn);
            request.Headers.AddSignedHeaders(hmac.HeaderValues);
            request.Headers.AddSignature(hmac.Signature);

            return new HmacResult { Hmac = hmac, IsSuccess = true };
        }
        else
        {
            throw new MissingHeaderException();
        }
    }
}
