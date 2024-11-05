using System.Net.Http.Headers;
using HmacManager.Components;
using HmacManager.Headers;
using HmacManager.Mvc;

namespace HmacManager.Extensions;

internal static class HttpRequestHeadersExtensions
{
    public static void AddRange(this HttpRequestHeaders source, IReadOnlyCollection<HeaderValue> headers)
    {
        foreach (var header in headers)
        {
            source.Add(header.Name, header.Value);
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
            var schemeHeaders = headerScheme.Headers;
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
        int maxAgeInSeconds,
        out Hmac value
    )
    {
        var parser = new HmacHeaderParser(headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault()));
        if (headers.TryGetHmacOptionsHeader(out var hmacOptions))
        {
            parser = new HmacOptionsHeaderParser(headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault()));
        }

        var signature = parser.GetAuthorization();
        var dateRequested = parser.GetDateRequested();
        var nonce = parser.GetNonce();
        var policy = parser.GetPolicy();
        var scheme = parser.GetScheme();

        if (dateRequested.HasValidDateRequested(maxAgeInSeconds))
        {
            if (headerScheme is null)
            {
                value = new Hmac
                {
                    Policy = policy,
                    HeaderScheme = scheme,
                    Signature = signature ?? string.Empty,
                    DateRequested = dateRequested,
                    Nonce = nonce,
                    HeaderValues = []
                };
            }
            else if (TryParseHeaders(headers, headerScheme, out var headerValues))
            {
                value = new Hmac
                {
                    Policy = policy,
                    HeaderScheme = scheme,
                    Signature = signature ?? string.Empty,
                    DateRequested = dateRequested,
                    Nonce = nonce,
                    HeaderValues = headerValues.ToArray()
                };
            }
            else
            {
                value = default!;
            }
        }
        else
        {
            value = default!;
        }

        return value is not null;
    }

    public static bool TryGetHmacOptionsHeader(
        this HttpRequestHeaders headers, 
        out string? value
    )
    {
        if (headers.TryGetValues(HmacAuthenticationDefaults.Headers.Options, out var values))
        {
            value = values.First();
            return true;
        }

        value = default;
        return false;
    }
}