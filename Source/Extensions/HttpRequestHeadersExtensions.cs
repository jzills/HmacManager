using System.Net.Http.Headers;
using Souce.Mvc;
using Source.Components;

namespace Source.Extensions;

internal static class HttpRequestHeadersExtensions
{
    public static void AddSignature(
        this HttpRequestHeaders headers, 
        string signature
    ) => headers.Authorization = new AuthenticationHeaderValue(
            HMACAuthenticationDefaults.Scheme, 
            signature
        );

    public static void AddRequestedOn(
        this HttpRequestHeaders headers, 
        DateTimeOffset requestedOn
    ) => headers.Add(HMACAuthenticationDefaults.Headers.RequestedOn, requestedOn.ToString());

    public static void AddNonce(
        this HttpRequestHeaders headers, 
        Guid nonce
    ) => headers.Add(HMACAuthenticationDefaults.Headers.Nonce, nonce.ToString());

    public static void AddMessageContent(
        this HttpRequestHeaders headers, 
        MessageContent[]? messageContent
    )
    {
        if (messageContent is MessageContent[] contentValues)
        {
            foreach (var content in contentValues)
            {
                headers.Add(content.Header!, content.Value);
            }
        }
    }

    public static bool TryParseHMAC(
        this HttpRequestHeaders headers,
        string[] messageContentHeaders,
        out HMAC value
    )
    {
        var hasRequiredAuthorizationHeader  = headers.HasRequiredAuthorizationHeader(out var signature);
        var hasRequiredRequestedOnHeader    = headers.HasRequiredRequestedOnHeader(out var requestedOn);
        var hasRequiredNonceHeader          = headers.HasRequiredNonceHeader(out var nonce);
        
        var contentHeaders = new List<MessageContent>(headers.Count());
        foreach (var messageContentHeader in messageContentHeaders)
        {
            if (headers.TryGetValues(messageContentHeader, out var contentHeaderValue))
            {
                var headerValue = contentHeaderValue.FirstOrDefault();
                contentHeaders.Add(new MessageContent
                {
                    Header = messageContentHeader,
                    Value = headerValue
                });
            }
        }
        
        value = new HMAC
        {
            Signature = signature,
            RequestedOn = requestedOn,
            Nonce = nonce,
            MessageContent = contentHeaders.ToArray()
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
            headers?.Authorization?.Scheme == HMACAuthenticationDefaults.Scheme &&
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
        if (headers.TryGetValues(HMACAuthenticationDefaults.Headers.RequestedOn, out var requestOnValues))
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
        if (headers.TryGetValues(HMACAuthenticationDefaults.Headers.Nonce, out var nonceValues))
        {
            return Guid.TryParse(nonceValues.FirstOrDefault(), out nonce);
        }

        nonce = default;
        return false;
    }
}