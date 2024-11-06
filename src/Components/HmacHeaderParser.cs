using HmacManager.Exceptions;
using HmacManager.Mvc;

namespace HmacManager.Components;

/// <summary>
/// A class representing a parser to extract HMAC headers from a request.
/// </summary>
public class HmacHeaderParser : IHmacHeaderParser
{
    /// <summary>
    /// A dictionary of headers and their corresponding values.
    /// </summary>
    protected readonly IDictionary<string, string> Headers;

    /// <summary>
    /// Creates an instance of <c>HmacHeaderParser</c>.
    /// </summary>
    public HmacHeaderParser()
    {
        Headers = new Dictionary<string, string>();
    }

    /// <summary>
    /// Initializes a new instance of the <c>HmacHeaderParser</c> class with the specified headers.
    /// </summary>
    /// <param name="headers">A dictionary of headers to parse.</param>
    public HmacHeaderParser(IDictionary<string, string> headers) => Headers = headers;

    /// <summary>
    /// Gets the policy from the headers.
    /// </summary>
    /// <returns>The policy string if found; otherwise, null.</returns>
    public virtual string GetAuthorization()
    {
        if (Headers.TryGetValue(HmacAuthenticationDefaults.Headers.Authorization, out var hmacAuthorizationHeader))
        {
            var hmacAuthorizationHeaderValues = hmacAuthorizationHeader?.Split(" ");
            if (hmacAuthorizationHeaderValues?.Length == 2 && 
                hmacAuthorizationHeaderValues[0] == HmacAuthenticationDefaults.AuthenticationScheme
            )
            {
                return hmacAuthorizationHeaderValues[1];
            }
            else
            {
                throw new BadHeaderFormatException();
            }
        }
        else
        {
            throw new MissingHeaderException();
        }
    }

    /// <summary>
    /// Gets the policy from the headers.
    /// </summary>
    /// <returns>The policy string if found; otherwise, null.</returns>
    public virtual string GetPolicy()
    {
        if (Headers.TryGetValue(HmacAuthenticationDefaults.Headers.Policy, out var policy))
        {
            if (!string.IsNullOrWhiteSpace(policy))
            {
                return policy;
            }
            else
            {
                throw new BadHeaderFormatException();
            }
        }
        else
        {
            throw new MissingHeaderException();
        }
    }

    /// <summary>
    /// Gets the scheme from the headers.
    /// </summary>
    /// <returns>The scheme string if found; otherwise, null.</returns>
    public virtual string? GetScheme()
    {
        Headers.TryGetValue(HmacAuthenticationDefaults.Headers.Scheme, out var scheme);
        return scheme;
    }

    /// <summary>
    /// Gets the nonce from the headers.
    /// </summary>
    /// <returns>The nonce as a Guid if found and valid; otherwise, null.</returns>
    public virtual Guid GetNonce()
    {
        if (Headers.TryGetValue(HmacAuthenticationDefaults.Headers.Nonce, out var nonce))
        {
            if (Guid.TryParse(nonce, out var nonceGuid))
            {
                return nonceGuid;
            }
            else
            {
                throw new BadHeaderFormatException();
            }
        }
        else
        {
            throw new MissingHeaderException();
        }
    }

    /// <summary>
    /// Gets the date requested from the headers.
    /// </summary>
    /// <returns>The date requested as a <c>DateTimeOffset</c> if found and valid; otherwise, null.</returns>
    public virtual DateTimeOffset GetDateRequested()
    {
        if (Headers.TryGetValue(HmacAuthenticationDefaults.Headers.DateRequested, out var dateRequested))
        {
            if (long.TryParse(dateRequested, out var dateRequestedInMilliseconds))
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(dateRequestedInMilliseconds);
            }
            else
            {
                throw new BadHeaderFormatException();
            }
        }
        else
        {
            throw new MissingHeaderException();
        }
    }

    /// <inheritdoc/>
    public virtual IHmacHeaderParser CreateParser(IDictionary<string, string> headers) => new HmacHeaderParser(headers);

    /// <summary>
    /// Parses the headers and creates an <c>Hmac</c> object if all required values are present.
    /// </summary>
    /// <returns>An <c>Hmac</c> object if parsing is successful; otherwise, throws <c>MissingHeaderException</c>.</returns>
    /// <exception cref="MissingHeaderException">Thrown if any required headers are missing.</exception>
    public virtual HmacPartial Parse(out string signature)
    {
        signature = GetAuthorization();

        return new HmacPartial
        {
            Policy = GetPolicy(),
            HeaderScheme = GetScheme(),
            Nonce = GetNonce(),
            DateRequested = GetDateRequested()
        };
    }
}