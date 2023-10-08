using HmacManagement.Caching;
using HmacManagement.Exceptions;
using HmacManagement.Extensions;
using HmacManagement.Remodel;

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
        if (request.Headers.TryParseHmac(_options.HeaderScheme, out var hmac))
        {
            if (await HasValidPreChecksAsync(hmac))
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
            var hmac = await GenerateHmacAsync(request, headerValues.ToArray());
            AddHmacHeaders(request, hmac);

            return new HmacResult { Hmac = hmac, IsSuccess = true };
        }
        else
        {
            throw new MissingHeaderException();
        }
    }

    private async Task<Hmac> GenerateHmacAsync(
        HttpRequestMessage request, 
        HeaderValue[]? headerValues = null
    )
    {
        var hmac = new Hmac { HeaderValues = headerValues };

        hmac.SigningContent = await _provider.ComputeSigningContentAsync(
            request, 
            hmac.RequestedOn, 
            hmac.Nonce,
            hmac.HeaderValues
        );

        hmac.Signature = _provider.ComputeSignature(
            hmac.SigningContent
        );

        return hmac;
    }

    private void AddHmacHeaders(HttpRequestMessage request, Hmac hmac)
    {
        request.Headers.AddSignature(hmac.Signature);
        request.Headers.AddRequestedOn(hmac.RequestedOn);
        request.Headers.AddNonce(hmac.Nonce);
        request.Headers.AddSignedHeaders(hmac.HeaderValues);
    }

    private async Task<bool> HasValidPreChecksAsync(Hmac hmac)
    {
        var hasValidPrechecks = 
            HasValidRequestedOn(hmac.RequestedOn) && await 
            HasValidNonceAsync (hmac.Nonce);

        return hasValidPrechecks;
    }

    private bool HasValidRequestedOn(DateTimeOffset requestedOn) => 
        DateTimeOffset.UtcNow.Subtract(requestedOn) < _options.MaxAge;

    private async Task<bool> HasValidNonceAsync(Guid nonce) => 
        !(await _cache.ContainsAsync(nonce));
}
