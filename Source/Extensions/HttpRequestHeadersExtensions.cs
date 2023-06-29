using System;
using System.Linq;
using System.Net.Http.Headers;
using Source.Components;

namespace Source.Extensions;

internal static class HttpRequestHeadersExtensions
{
    public static void AddSignature(
        this HttpRequestHeaders headers, 
        string signature
    ) => headers.Authorization = new AuthenticationHeaderValue("Bearer", signature);

    public static void AddRequestedOn(
        this HttpRequestHeaders headers, 
        DateTimeOffset requestedOn
    ) => headers.Add("X-Requested-On", requestedOn.ToString());

    public static void AddNonce(
        this HttpRequestHeaders headers, 
        Guid nonce
    ) => headers.Add("X-Nonce", nonce.ToString());

    public static bool HasRequiredHeaders(
        this HttpRequestHeaders headers,
        out RequiredHeaderValues headerValues
    )
    {
        var hasRequiredAuthorizationHeader  = headers.HasRequiredAuthorizationHeader(out var signature);
        var hasRequiredRequestedOnHeader    = headers.HasRequiredRequestedOnHeader(out var requestedOn);
        var hasRequiredNonceHeader          = headers.HasRequiredNonceHeader(out var nonce);
        
        headerValues = new RequiredHeaderValues
        {
            Signature = signature,
            RequestedOn = requestedOn,
            Nonce = nonce
        };

        return hasRequiredAuthorizationHeader && 
            hasRequiredRequestedOnHeader && 
            hasRequiredNonceHeader;
    }

    public static bool HasRequiredAuthorizationHeader(
        this HttpRequestHeaders headers, 
        out string? signature
    )
    {
        var hasValidHeader = 
            headers?.Authorization?.Scheme == "Bearer" &&
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
        if (headers.TryGetValues("X-Requested-On", out var requestOnValues))
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
        if (headers.TryGetValues("X-Nonce", out var nonceValues))
        {
            return Guid.TryParse(nonceValues.FirstOrDefault(), out nonce);
        }

        nonce = default;
        return false;
    }
}