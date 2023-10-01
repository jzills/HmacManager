using HmacManagement.Caching;
using HmacManagement.Exceptions;
using HmacManagement.Extensions;
using HmacManagement.Policies;

namespace HmacManagement.Components;

public class HmacManager : IHmacManager
{
    private readonly HmacManagerOptions _options;
    private readonly INonceCache _cache;
    private readonly IHmacProvider _provider;
    private readonly ISigningPolicyCollection _signingPolicies;

    public HmacManager(
        HmacManagerOptions options,
        INonceCache cache,
        IHmacProvider provider,
        ISigningPolicyCollection signingPolicies
    )
    {
        _options = options;
        _cache = cache;
        _provider = provider;
        _signingPolicies = signingPolicies;
    }

    public async Task<HmacResult> VerifyAsync(HttpRequestMessage request, string signingPolicy)
    {
        var policy = _signingPolicies.GetPolicy(signingPolicy);
        if (request.Headers.TryParseHmac(policy.HeaderClaimMappings.GetRequiredHeaders(), out var hmac))
        {
            var hasValidPrechecks = 
                HasValidRequestedOn(hmac.RequestedOn) && await 
                HasValidNonceAsync (hmac.Nonce);

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

    public async Task<HmacResult> SignAsync(HttpRequestMessage request, string signingPolicy)
    {
        var policy = _signingPolicies.GetPolicy(signingPolicy);
        var headers = policy.HeaderClaimMappings.GetRequiredHeaders();
        // Check headers all exist in HttpRequestMessage

        if (request.Headers.TryParseRequiredHeaders(headers, out var headersToSign))
        {   
            var hmac = new Hmac { SignedHeaders = headersToSign.ToArray() };

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
        else
        {
            throw new MissingHeaderException();
        }
    }

    private bool HasValidRequestedOn(DateTimeOffset requestedOn) => 
        DateTimeOffset.UtcNow.Subtract(requestedOn) < _options.MaxAge;

    private async Task<bool> HasValidNonceAsync(Guid nonce) => 
        !(await _cache.ContainsAsync(nonce));

    public Task<HmacResult> SignAsync(HttpRequestMessage request)
    {
        throw new NotImplementedException();
    }

    public Task<HmacResult> VerifyAsync(HttpRequestMessage request)
    {
        throw new NotImplementedException();
    }
}
