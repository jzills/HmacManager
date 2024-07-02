using HmacManager.Extensions;
using HmacManager.Headers;

namespace HmacManager.Components;

public class HmacFactory : IHmacFactory
{
    private readonly IHmacProvider _hmacProvider;

    public HmacFactory(IHmacProvider hmacProvider)
    {
        _hmacProvider = hmacProvider;
    }

    public async Task<Hmac?> CreateAsync(HttpRequestMessage request, HeaderScheme? headerScheme = null)
    {
        if (request.Headers.TryParseHeaders(headerScheme, out var headerValues))
        {   
            var hmac = new Hmac { HeaderValues = headerValues.ToArray() };

            // Generate the formatted signing content based on
            // the provided hmac values
            hmac.SigningContent = await _hmacProvider
                .ComputeSigningContentAsync(request, 
                    hmac.DateRequested, 
                    hmac.Nonce,
                    hmac.HeaderValues
                );

            // Compute the signature against the signing content
            hmac.Signature = await _hmacProvider
                .ComputeSignatureAsync(hmac.SigningContent);

            return hmac;
        }
        else
        {
            return null;
        }
    }

    public async Task<Hmac?> CreateAsync(HttpRequestMessage request, Hmac? hmac)
    {
        if (hmac is not null)
        {
            hmac.SigningContent = await _hmacProvider
                .ComputeSigningContentAsync(request, 
                    hmac.DateRequested, 
                    hmac.Nonce,
                    hmac.HeaderValues
                );

            var signature = await _hmacProvider
                .ComputeSignatureAsync(hmac.SigningContent);

            return new Hmac
            {
                DateRequested = hmac.DateRequested,
                HeaderValues = hmac.HeaderValues,
                Nonce = hmac.Nonce,
                Signature = signature,
                SigningContent = hmac.SigningContent
            };
        }
        else
        {
            return null;
        }
    }
}