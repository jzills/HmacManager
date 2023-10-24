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
        HeaderScheme? headerScheme,
        out IReadOnlyCollection<HeaderValue> headerValues
    )
    {
        if (headerScheme is null)
        {
            headerValues = new List<HeaderValue>().AsReadOnly();
            return true;
        }
        else
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
    }

    public static bool TryParseHmac(
        this HttpRequestHeaders headers,
        HeaderScheme? headerScheme,
        TimeSpan maxAge,
        out Hmac value
    )
    {
        var hasAuthorizationHeader  = headers.TryGetAuthorizationHeader(out var signature);
        var hasRequestedOnHeader    = headers.TryGetRequestedOnHeader(out var requestedOn) && requestedOn.HasValidRequestedOn(maxAge);
        var hasNonceHeader          = headers.TryGetNonceHeader(out var nonce);
        
        if (headerScheme is null)
        {
            value = new Hmac
            {
                Signature = signature ?? string.Empty,
                RequestedOn = requestedOn,
                Nonce = nonce,
                HeaderValues = new HeaderValue[] { }
            };
        }
        else if (TryParseHeaders(headers, headerScheme, out var headerValues))
        {
            value = new Hmac
            {
                Signature = signature ?? string.Empty,
                RequestedOn = requestedOn,
                Nonce = nonce,
                HeaderValues = headerValues.ToArray()
            };
        }
        else
        {
            value = new Hmac() { RequestedOn = DateTime.MinValue };
        }

        return 
            hasAuthorizationHeader && 
            hasRequestedOnHeader && 
            hasNonceHeader;
    }

    public static bool TryGetAuthorizationHeader(
        this HttpRequestHeaders headers, 
        out string? signature
    )
    {
        var hasValidHeader = 
            headers?.Authorization?.Scheme == HmacAuthenticationDefaults.AuthenticationScheme &&
            !string.IsNullOrEmpty(headers?.Authorization?.Parameter);
        
        if (hasValidHeader)
        {
            signature = headers!.Authorization!.Parameter;
        }
        else
        {
            signature = default;
        }

        return hasValidHeader;
    }

    public static bool TryGetRequestedOnHeader(
        this HttpRequestHeaders headers, 
        out DateTimeOffset requestedOn
    )
    {
        if (headers.TryGetValues(HmacAuthenticationDefaults.Headers.RequestedOn, out var value))
        {
            return DateTimeOffset.TryParse(
                value.FirstOrDefault(), 
                out requestedOn
            );
        }

        requestedOn = default;
        return false;
    }

    public static bool TryGetNonceHeader(
        this HttpRequestHeaders headers, 
        out Guid nonce
    )
    {
        if (headers.TryGetValues(HmacAuthenticationDefaults.Headers.Nonce, out var value))
        {
            return Guid.TryParse(value.FirstOrDefault(), out nonce);
        }

        nonce = default;
        return false;
    }
}