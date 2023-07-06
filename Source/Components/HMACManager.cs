using Source.Caching;
using Source.Exceptions;
using Source.Extensions;

namespace Source.Components;

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

    public async Task<VerificationResult> VerifyAsync(HttpRequestMessage request)
    {
        if (request.Headers.TryParseHMAC(
                _options.MessageContentHeaders, out var hmac))
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

                var signingContent = await _provider.ComputeSigningContentAsync(
                    request, 
                    hmac.RequestedOn, 
                    hmac.Nonce,
                    hmac.MessageContent
                );

                var signature = _provider.ComputeSignature(signingContent);
                return new VerificationResult
                {
                    HMAC = hmac,
                    IsTrusted = signature == hmac.Signature
                };
            }
        }

        return new VerificationResult();
    }

    public async Task<SigningResult> SignAsync(
        HttpRequestMessage request, 
        MessageContent[]? messageContent = null
    )
    {
        MissingHeaderException.ThrowIfMissing(
            _options.MessageContentHeaders,
            messageContent
        );

        var hmac = new HMAC { MessageContent = messageContent };

        hmac.SigningContent = await _provider.ComputeSigningContentAsync(
            request, 
            hmac.RequestedOn, 
            hmac.Nonce,
            hmac.MessageContent
        );

        hmac.Signature = _provider.ComputeSignature(
            hmac.SigningContent
        );

        request.Headers.AddSignature(hmac.Signature);
        request.Headers.AddRequestedOn(hmac.RequestedOn);
        request.Headers.AddNonce(hmac.Nonce);
        request.Headers.AddAdditionalContent(hmac.MessageContent);

        return new SigningResult
        {
            HMAC = hmac,
            IsSigned = true
        };
    }

    public bool HasValidRequestedOn(DateTimeOffset requestedOn) => 
        DateTimeOffset.UtcNow.Subtract(requestedOn) < _options.MaxAge;

    public async Task<bool> HasValidNonceAsync(Guid nonce) => !(await _cache.ContainsAsync(nonce));
}