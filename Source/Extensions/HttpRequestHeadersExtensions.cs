using System.Net.Http.Headers;
using HmacManagement.Components;
using HmacManagement.Headers;
using HmacManagement.Mvc;

namespace HmacManagement.Extensions;

internal static class HttpRequestHeadersExtensions
{
    public static void AddSignature(
        this HttpRequestHeaders headers, 
        string signature
    ) => headers.Authorization = new AuthenticationHeaderValue(
            HmacAuthenticationDefaults.AuthenticationScheme, 
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
        HeaderValue[]? signedHeaders
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

    public static bool TryParseHeaders(
        this HttpRequestHeaders headers,
        HeaderScheme headerScheme,
        out IReadOnlyCollection<HeaderValue> headerValues
    )
    {
        var schemeHeaders = headerScheme.GetRequiredHeaders();
        var schemeHeaderValues = new List<HeaderValue>(schemeHeaders.Count);
        foreach (var schemeHeader in schemeHeaders)
        {
            if (headers.TryGetValues(schemeHeader.Name, out var values))
            {
                var schemeHeaderValue = values.First();
                schemeHeaderValues.Add(new HeaderValue(
                    schemeHeader.Name, 
                    schemeHeader.ClaimType, 
                    schemeHeaderValue
                ));
            }
            else
            {
                headerValues = new List<HeaderValue>().AsReadOnly();
                return false;
            }
        }

        headerValues = schemeHeaderValues.AsReadOnly();
        return true;
    }

    public static bool TryParseHmac(
        this HttpRequestHeaders headers,
        HeaderScheme headerScheme,
        out Hmac value
    )
    {
        var hasRequiredAuthorizationHeader  = headers.HasRequiredAuthorizationHeader(out var signature);
        var hasRequiredRequestedOnHeader    = headers.HasRequiredRequestedOnHeader(out var requestedOn);
        var hasRequiredNonceHeader          = headers.HasRequiredNonceHeader(out var nonce);
        
        var schemeHeaders = headerScheme.GetRequiredHeaders();
        var schemeHeaderValues = new List<HeaderValue>(schemeHeaders.Count);
        foreach (var schemeHeader in schemeHeaders)
        {
            if (headers.TryGetValues(schemeHeader.Name, out var values))
            {
                var schemeHeaderValue = values.First();
                schemeHeaderValues.Add(new HeaderValue(
                    schemeHeader.Name, 
                    schemeHeader.ClaimType, 
                    schemeHeaderValue
                ));
            }
        }
        
        value = new Hmac
        {
            Signature = signature,
            RequestedOn = requestedOn,
            Nonce = nonce,
            HeaderValues = schemeHeaderValues.ToArray()
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
            headers?.Authorization?.Scheme == HmacAuthenticationDefaults.AuthenticationScheme &&
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