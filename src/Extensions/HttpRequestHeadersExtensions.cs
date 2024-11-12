using System.Net.Http.Headers;
using HmacManager.Headers;

namespace HmacManager.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpRequestHeaders"/>.
/// </summary>
internal static class HttpRequestHeadersExtensions
{
    /// <summary>
    /// Adds a collection of headers to the <see cref="HttpRequestHeaders"/>.
    /// </summary>
    /// <param name="source">The <see cref="HttpRequestHeaders"/> to which the headers will be added.</param>
    /// <param name="headers">The collection of <see cref="HeaderValue"/> to be added to the headers.</param>
    internal static void AddRange(this HttpRequestHeaders source, IReadOnlyCollection<HeaderValue> headers)
    {
        foreach (var header in headers)
        {
            source.Add(header.Name, header.Value);
        }
    }

    /// <summary>
    /// Tries to parse the headers based on the provided <see cref="HeaderScheme"/>.
    /// </summary>
    /// <param name="headers">The <see cref="HttpRequestHeaders"/> to be parsed.</param>
    /// <param name="headerScheme">The <see cref="HeaderScheme"/> used to define which headers to look for.</param>
    /// <param name="headerValues">The parsed <see cref="IReadOnlyCollection{HeaderValue}"/> containing the header values, if successful.</param>
    /// <returns><c>true</c> if the headers were successfully parsed according to the scheme; otherwise, <c>false</c>.</returns>
    internal static bool TryParseHeaders(
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
}