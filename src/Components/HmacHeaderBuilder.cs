using HmacManager.Exceptions;
using HmacManager.Extensions;
using HmacManager.Headers;
using HmacManager.Mvc;

namespace HmacManager.Components;

/// <summary>
/// A class representing a builder to create required hmac headers to be added to a request.
/// </summary> 
public class HmacHeaderBuilder
{
    /// <summary>
    /// A dictionary of <see cref="HmacAuthenticationDefaults.Headers"/> defaulting to null.
    /// </summary> 
    protected readonly IDictionary<string, string?> HeaderValues = new Dictionary<string, string?>
    {
        { HmacAuthenticationDefaults.Headers.Policy, null },
        { HmacAuthenticationDefaults.Headers.Scheme, null },
        { HmacAuthenticationDefaults.Headers.Nonce, null },
        { HmacAuthenticationDefaults.Headers.DateRequested, null }
    };

    /// <summary>
    /// An enumerable of header values that are non empty, i.e. both keys and values are not null, empty or whitespace.
    /// </summary>
    protected IEnumerable<KeyValuePair<string, string?>> NonEmptyHeaderValues => HeaderValues.Where(element => element.NonEmpty());

    /// <summary>
    /// Creates an instance of <c>HmacHeaderBuilder</c>.
    /// </summary>
    /// <param name="options">An instance of <c>HmacManagerOptions</c>.</param>
    /// <param name="hmac">An instance of <c>Hmac</c>.</param>
    public HmacHeaderBuilder(HmacManagerOptions options, Hmac hmac)
    {
        WithPolicy(options.Policy);
        
        if (!string.IsNullOrWhiteSpace(options.HeaderScheme?.Name))
        {
            WithScheme(options.HeaderScheme.Name);
        }

        WithNonce(hmac.Nonce);
        WithDateRequested(hmac.DateRequested);
    }

    /// <summary>
    /// Adds the specified policy to the builder.
    /// </summary>
    /// <param name="policy">A string representing the policy.</param>
    /// <returns>The builder.</returns>    
    public HmacHeaderBuilder WithPolicy(string policy)
    {
        HeaderValues[HmacAuthenticationDefaults.Headers.Policy] = policy;
        return this;
    }
    
    /// <summary>
    /// Adds the specified scheme to the builder.
    /// </summary>
    /// <param name="scheme">A string representing the scheme.</param>
    /// <returns>The builder.</returns> 
    public HmacHeaderBuilder WithScheme(string scheme)
    {
        HeaderValues[HmacAuthenticationDefaults.Headers.Scheme] = scheme;
        return this;
    }

    /// <summary>
    /// Adds the specified nonce to the builder.
    /// </summary>
    /// <param name="nonce">The nonce represented by a guid.</param>
    /// <returns>The builder.</returns> 
    public HmacHeaderBuilder WithNonce(Guid nonce)
    {
        HeaderValues[HmacAuthenticationDefaults.Headers.Nonce] = nonce.ToString();
        return this;
    }

    /// <summary>
    /// Adds the specified date and time to the builder.
    /// </summary>
    /// <param name="dateRequested">The date requested.</param>
    /// <returns>The builder.</returns>
    public HmacHeaderBuilder WithDateRequested(DateTimeOffset dateRequested)
    {
        HeaderValues[HmacAuthenticationDefaults.Headers.DateRequested] = dateRequested.ToUnixTimeMilliseconds().ToString();
        return this;
    }

    /// <summary>
    /// Builds the collection of header values.
    /// </summary>
    /// <returns>A readonly collection of header values.</returns>
    public virtual IReadOnlyCollection<HeaderValue> Build()
    {
        if (NonEmptyHeaderValues.Any())
        {
            return NonEmptyHeaderValues.Select(header => new HeaderValue(header)).ToList();
        }
        else
        {
            throw new MissingHeaderException();
        }
    }
}