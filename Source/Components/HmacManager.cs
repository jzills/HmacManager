using HmacManagement.Caching;
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
        if (request.Headers.TryParseHmac(_options.SignedHeaders, out var hmac))
        {
            var hasValidPrechecks = 
                HasValidRequestedOn(hmac.RequestedOn) && await 
                HasValidNonceAsync(hmac.Nonce);

            if (hasValidPrechecks)
            {   
                await _cache.SetAsync(
                    hmac.Nonce, 
                    hmac.RequestedOn
                );

                hmac.SigningContent = await _provider.ComputeSigningContentAsync(
                    request, 
                    hmac.RequestedOn, 
                    hmac.Nonce,
                    hmac.SignedHeaders
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

    public async Task<HmacResult> SignAsync(
        HttpRequestMessage request, 
        Header[]? signedHeaders = null
    )
    {
        MissingHeaderException.ThrowIfMissing(
            _options.SignedHeaders,
            signedHeaders
        );

        var hmac = new Hmac { SignedHeaders = signedHeaders };

        hmac.SigningContent = await _provider.ComputeSigningContentAsync(
            request, 
            hmac.RequestedOn, 
            hmac.Nonce,
            hmac.SignedHeaders
        );

        hmac.Signature = _provider.ComputeSignature(
            hmac.SigningContent
        );

        request.Headers.AddSignature(hmac.Signature);
        request.Headers.AddRequestedOn(hmac.RequestedOn);
        request.Headers.AddNonce(hmac.Nonce);
        request.Headers.AddSignedHeaders(hmac.SignedHeaders);

        return new HmacResult { Hmac = hmac, IsSuccess = true };
    }

    private bool HasValidRequestedOn(DateTimeOffset requestedOn) => 
        DateTimeOffset.UtcNow.Subtract(requestedOn) < _options.MaxAge;

    private async Task<bool> HasValidNonceAsync(Guid nonce) => 
        !(await _cache.ContainsAsync(nonce));
}
