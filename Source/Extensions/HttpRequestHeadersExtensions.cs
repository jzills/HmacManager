using System.Net.Http.Headers;
using HmacManagement.Components;
using HmacManagement.Headers;
using HmacManagement.Mvc;

namespace HmacManagement.Extensions;

internal static class HttpRequestHeadersExtensions
{
    public static void AddSignature(
        this HttpRequestHeaders headers, 
        string signature,
        string policy,
        string? headerScheme = null
    )
    {
        ArgumentNullException.ThrowIfNullOrEmpty(signature, nameof(signature));
        ArgumentNullException.ThrowIfNullOrEmpty(policy, nameof(policy));

        if (string.IsNullOrEmpty(headerScheme))
        {
            headers.Authorization = new AuthenticationHeaderValue(
                $"{HmacAuthenticationDefaults.AuthenticationScheme}-{policy}", 
                signature
            );
        }
        else
        {
            headers.Authorization = new AuthenticationHeaderValue(
                $"{HmacAuthenticationDefaults.AuthenticationScheme}-{policy}-{headerScheme}", 
                signature
            );
        }
    }

    public static void AddDateRequested(
        this HttpRequestHeaders headers, 
        DateTimeOffset dateRequested
    ) => headers.Add(HmacAuthenticationDefaults.Headers.DateRequested, dateRequested.ToString());

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
            var schemeHeaders = headerScheme.GetHeaders();
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
        var hasDateRequestedHeader  = headers.TryGetDateRequestedHeader(out var dateRequested) && dateRequested.HasValidDateRequested(maxAge);
        var hasNonceHeader          = headers.TryGetNonceHeader(out var nonce);
        
        if (headerScheme is null)
        {
            value = new Hmac
            {
                Signature = signature ?? string.Empty,
                DateRequested = dateRequested,
                Nonce = nonce,
                HeaderValues = new HeaderValue[] { }
            };
        }
        else if (TryParseHeaders(headers, headerScheme, out var headerValues))
        {
            value = new Hmac
            {
                Signature = signature ?? string.Empty,
                DateRequested = dateRequested,
                Nonce = nonce,
                HeaderValues = headerValues.ToArray()
            };
        }
        else
        {
            value = new Hmac() { DateRequested = DateTime.MinValue };
        }

        return 
            hasAuthorizationHeader && 
            hasDateRequestedHeader && 
            hasNonceHeader;
    }

    public static bool TryGetAuthorizationHeader(
        this HttpRequestHeaders headers, 
        out string? signature
    )
    {
        var hasValidAuthorizationHeader = 
            headers.Authorization is not null && 
            headers.Authorization.Scheme.StartsWith(HmacAuthenticationDefaults.AuthenticationScheme);
        
        if (hasValidAuthorizationHeader)
        {
            signature = headers.Authorization!.Parameter;
        }
        else
        {
            signature = default;
        }

        return hasValidAuthorizationHeader;
    }

    public static bool TryGetDateRequestedHeader(
        this HttpRequestHeaders headers, 
        out DateTimeOffset dateRequested
    )
    {
        if (headers.TryGetValues(HmacAuthenticationDefaults.Headers.DateRequested, out var value))
        {
            return DateTimeOffset.TryParse(
                value.FirstOrDefault(), 
                out dateRequested
            );
        }

        dateRequested = default;
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