using HmacManager.Caching;
using HmacManager.Caching.Extensions;
using HmacManager.Extensions;

namespace HmacManager.Components;

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
                    hmac.DateRequested
                );

                hmac.SigningContent = await _provider.ComputeSigningContentAsync(
                    request, 
                    hmac.DateRequested, 
                    hmac.Nonce,
                    hmac.HeaderValues
                );

                var signature = _provider.ComputeSignature(hmac.SigningContent);

                return new HmacResult
                {
                    Policy = _options.Policy,
                    HeaderScheme = _options.HeaderScheme?.Name!,
                    Hmac = hmac,
                    IsSuccess = signature == hmac.Signature
                };
            }
        }
        
        return new HmacResult 
        { 
            Policy = _options.Policy,
            HeaderScheme = _options.HeaderScheme?.Name!,
            Hmac = null, 
            IsSuccess = false
        };
    }

    public async Task<HmacResult> SignAsync(HttpRequestMessage request)
    {
        if (request.Headers.TryParseHeaders(_options.HeaderScheme, out var headerValues))
        {   
            var hmac = new Hmac { HeaderValues = headerValues.ToArray() };

            // Generate the formatted signing content based on
            // the provided hmac values
            hmac.SigningContent = await _provider.ComputeSigningContentAsync(
                request, 
                hmac.DateRequested, 
                hmac.Nonce,
                hmac.HeaderValues
            );

            // Compute the signature against the signing content
            hmac.Signature = _provider.ComputeSignature(
                hmac.SigningContent
            );

            // Add required headers to the request
            request.Headers.AddNonce(hmac.Nonce);
            request.Headers.AddDateRequested(hmac.DateRequested);

            // Do not add signed headers here...
            // The new process flow requires users to
            // add the headers prior to this signing call.
            // request.Headers.AddSignedHeaders(hmac.HeaderValues);
            
            request.Headers.AddSignature(
                hmac.Signature, 
                _options.Policy, 
                _options.HeaderScheme?.Name
            );

            return new HmacResult 
            { 
                Policy = _options.Policy,
                HeaderScheme = _options.HeaderScheme?.Name!,
                Hmac = hmac, 
                IsSuccess = true
            };
        }
        else
        {
            return new HmacResult 
            { 
                Policy = _options.Policy,
                HeaderScheme = _options.HeaderScheme?.Name!,
                Hmac = null, 
                IsSuccess = false
            };
        }
    }
}
