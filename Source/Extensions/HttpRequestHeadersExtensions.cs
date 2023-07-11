using System.Net.Http.Headers;
using HmacManager.Components;
using HmacManager.Mvc;

namespace HmacManager.Extensions;

internal static class HttpRequestHeadersExtensions
{
    public static void AddSignature(
        this HttpRequestHeaders headers, 
        string signature
    ) => headers.Authorization = new AuthenticationHeaderValue(
            HmacAuthenticationDefaults.Scheme, 
            signature
        );

    public static void AddRequestedOn(
        this HttpRequestHeaders headers, 
        DateTimeOffset requestedOn
    ) => headers.Add(HmacAuthenticationDefaults.Headers.RequestedOn, requestedOn.ToString());

    public static void AddNonce(
        this HttpRequestHeaders headers, 
        Guid nonce
    ) => headers.Add(HmacAuthenticationDefaults.Headers.Nonce, nonce.ToString());

    public static void AddSignedHeaders(
        this HttpRequestHeaders headers, 
        Header[]? signedHeaders
    )
    {
        if (signedHeaders is not null)
        {
            foreach (var signedHeader in signedHeaders)
            {
                headers.Add(signedHeader.Name!, signedHeader.Value);
            }
        }
    }

    public static bool TryParseHmac(
        this HttpRequestHeaders headers,
        string[] headersToVerify,
        out Hmac value
    )
    {
        var hasRequiredAuthorizationHeader  = headers.HasRequiredAuthorizationHeader(out var signature);
        var hasRequiredRequestedOnHeader    = headers.HasRequiredRequestedOnHeader(out var requestedOn);
        var hasRequiredNonceHeader          = headers.HasRequiredNonceHeader(out var nonce);
        
        var headersToSign = new List<Header>(headers.Count());
        foreach (var headerToVerify in headersToVerify)
        {
            if (headers.TryGetValues(headerToVerify, out var contentHeaderValue))
            {
                var headerValue = contentHeaderValue.FirstOrDefault();
                headersToSign.Add(new Header
                {
                    Name = headerToVerify,
                    Value = headerValue
                });
            }
        }
        
        value = new Hmac
        {
            Signature = signature,
            RequestedOn = requestedOn,
            Nonce = nonce,
            SignedHeaders = headersToSign.ToArray()
        };

        return 
            hasRequiredAuthorizationHeader && 
            hasRequiredRequestedOnHeader && 
            hasRequiredNonceHeader;
    }

    public static bool HasRequiredAuthorizationHeader(
        this HttpRequestHeaders headers, 
        out string? signature
    )
    {
        var hasValidHeader = 
            headers?.Authorization?.Scheme == HmacAuthenticationDefaults.Scheme &&
            !string.IsNullOrEmpty(headers?.Authorization?.Parameter);
        
        signature = hasValidHeader ? 
            headers!.Authorization!.Parameter :
            default;

        return hasValidHeader;
    }

    public static bool HasRequiredRequestedOnHeader(
        this HttpRequestHeaders headers, 
        out DateTimeOffset requestedOn
    )
    {
        if (headers.TryGetValues(HmacAuthenticationDefaults.Headers.RequestedOn, out var requestOnValues))
        {
            return DateTimeOffset.TryParse(
                requestOnValues.FirstOrDefault(), 
                out requestedOn
            );
        }

        requestedOn = default;
        return false;
    }

    public static bool HasRequiredNonceHeader(
        this HttpRequestHeaders headers, 
        out Guid nonce
    )
    {
        if (headers.TryGetValues(HmacAuthenticationDefaults.Headers.Nonce, out var nonceValues))
        {
            return Guid.TryParse(nonceValues.FirstOrDefault(), out nonce);
        }

        nonce = default;
        return false;
    }
}