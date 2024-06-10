using HmacManager.Caching;
using HmacManager.Caching.Extensions;
using HmacManager.Extensions;

namespace HmacManager.Components;

/// <summary>
/// A class representing a <c>HmacManager</c>.
/// </summary>
public class HmacManager : IHmacManager
{
    private readonly HmacManagerOptions _options;
    private readonly IHmacProvider _provider;
    private readonly INonceCache _cache;

    /// <summary>
    /// Creates a <c>HmacManager</c> object.
    /// </summary>
    /// <param name="options"><c>HmacManagerOptions</c></param>
    /// <param name="provider"><c>IHmacProvider</c></param>
    /// <param name="cache"><c>INonceCache</c></param>
    /// <returns>A <c>HmacManager</c> object.</returns>
    public HmacManager(
        HmacManagerOptions options,
        IHmacProvider provider,
        INonceCache cache
    )
    {
        _options = options;
        _provider = provider;
        _cache = cache;
    }

    /// <inheritdoc/>
    public async Task<HmacResult> VerifyAsync(HttpRequestMessage request)
    {
        if (request.Headers.TryParseHmac(_options.HeaderScheme, _options.MaxAgeInSeconds, out var hmac))
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

                var signature = await _provider.ComputeSignatureAsync(hmac.SigningContent);

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

    /// <inheritdoc/>
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
            hmac.Signature = await _provider.ComputeSignatureAsync(
                hmac.SigningContent
            );

            // Add required headers to the request
            request.Headers.AddNonce(hmac.Nonce);
            request.Headers.AddDateRequested(hmac.DateRequested);
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
