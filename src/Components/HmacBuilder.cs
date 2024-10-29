using HmacManager.Exceptions;
using HmacManager.Extensions;
using HmacManager.Headers;

namespace HmacManager.Components;

public class HmacBuilder
{
    protected readonly Hmac Hmac = new();
    protected readonly HttpRequestMessage Request;
    protected IHmacSignatureProvider Provider { get; set; }

    public HmacBuilder(HttpRequestMessage request, HeaderScheme? headerScheme = null)
    {
        if (request.Headers.TryParseHeaders(headerScheme, out var headerValues))
        {
            Hmac.HeaderValues = headerValues.ToArray();
        }
        else
        {
            throw new MissingHeaderException();
        }

        Request = request;
    }

    public HmacBuilder(HttpRequestMessage request, HmacPartial hmac)
    {
        Hmac = Hmac with 
        {
            DateRequested = hmac.DateRequested,
            Nonce = hmac.Nonce,
            HeaderValues = hmac.HeaderValues
        };

        Request = request;
    }

    public HmacBuilder WithProvider(IHmacSignatureProvider provider)
    {
        Provider = provider;
        return this;
    }

    public async Task<Hmac?> BuildAsync()
    {
        var signingContent = await Provider.ComputeSigningContentAsync(Request, 
            Hmac.DateRequested, 
            Hmac.Nonce,
            Hmac.HeaderValues
        );

        if (string.IsNullOrWhiteSpace(signingContent))
        {
            // throw
        }

        var signature = await Provider.ComputeSignatureAsync(signingContent);

        if (string.IsNullOrWhiteSpace(signature))
        {
            // throw
        }

        return Hmac with 
        { 
            SigningContent = signingContent, 
            Signature = signature
        };
    }
}