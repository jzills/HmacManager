using System.Net.Http.Headers;
using HmacManager.Caching;
using HmacManager.Caching.Extensions;
using HmacManager.Extensions;

namespace HmacManager.Components;

/// <summary>
/// A class representing a <see cref="HmacManager"/>.
/// </summary>
public class HmacManager : IHmacManager
{
    /// <inheritdoc/>
    public HmacManagerOptions Options { get; }

    /// <summary>
    /// An implementation of <see cref="IHmacFactory"/> for creating <see cref="Hmac"/> objects. 
    /// </summary>
    protected readonly IHmacFactory Factory;

    /// <summary>
    /// An implementation of <see cref="IHmacResultFactory"/> for creating <see cref="HmacResult"/> objects. 
    /// </summary>
    protected readonly IHmacResultFactory ResultFactory;

    /// <summary>
    /// An implementation of <see cref="INonceCache"/> for storing nonce values.
    /// </summary>
    protected readonly INonceCache Cache;

    /// <summary>
    /// Creates a <see cref="HmacManager"/> object.
    /// </summary>
    /// <param name="options"><see cref="HmacManagerOptions"/></param> 
    /// <param name="factory"><see cref="IHmacFactory"/></param>
    /// <param name="resultFactory"><see cref="IHmacResultFactory"/></param>
    /// <param name="cache"><see cref="INonceCache"/></param>
    /// <returns>A <see cref="HmacManager"/> object.</returns>
    public HmacManager(
        HmacManagerOptions options,
        IHmacFactory factory,
        IHmacResultFactory resultFactory,
        INonceCache cache
    )
    {
        Options = options;
        Factory = factory;
        ResultFactory = resultFactory;
        Cache = cache;
    }

    /// <inheritdoc/>
    public async Task<HmacResult> VerifyAsync(HttpRequestMessage request)
    {
        if (TryParseHmac(request.Headers, out var incomingHmac))
        {
            if (await IsValidAsync(incomingHmac))
            {   
                var hmacVerification = await Factory.CreateAsync(request, incomingHmac);
                if (hmacVerification.IsVerified(incomingHmac))
                {
                    return ResultFactory.Success(hmacVerification);
                }
            }
        }
        
        return ResultFactory.Failure();
    }

    /// <inheritdoc/>
    public async Task<HmacResult> SignAsync(HttpRequestMessage request)
    {
        var hmac = await Factory.CreateAsync(request, Options.Policy, Options.Scheme);
        if (hmac is not null)
        {
            var headers = Options.HeaderBuilder.CreateBuilder(Options, hmac).Build();
            request.Headers.AddRange(headers);
            
            return ResultFactory.Success(hmac);
        }
        else
        {
            return ResultFactory.Failure();
        }
    }

    /// <summary>
    /// Validates the provided HMAC partial object asynchronously to ensure it meets the criteria specified in the options.
    /// </summary>
    /// <param name="incomingHmac">The incoming <see cref="HmacPartial"/> to validate.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, with a result of <c>true</c> if the HMAC is valid; otherwise, <c>false</c>.
    /// </returns>
    private async Task<bool> IsValidAsync(HmacPartial? incomingHmac) =>
        incomingHmac is not null && 
        incomingHmac.DateRequested.HasValidDateRequested(Options.MaxAgeInSeconds) &&
            await Cache.IsValidNonceAsync(
                incomingHmac.Nonce,
                incomingHmac.DateRequested
            );

    /// <summary>
    /// Attempts to parse the provided HTTP request headers into an <see cref="Hmac"/> object.
    /// </summary>
    /// <param name="headers">The <see cref="HttpRequestHeaders"/> to parse.</param>
    /// <param name="value">
    /// When this method returns, contains the parsed <see cref="Hmac"/> object if parsing was successful;
    /// otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if parsing was successful and a valid <see cref="Hmac"/> object was created; otherwise, <c>false</c>.
    /// </returns>
    private bool TryParseHmac(HttpRequestHeaders headers, out Hmac? value)
    {
        var hmacPartial = Options.HeaderParser.CreateParser(headers).Parse(out var signature);
        if (Options.Scheme is null)
        {
            value = new Hmac
            {
                Policy = hmacPartial.Policy,
                Scheme = hmacPartial.Scheme,
                Signature = signature ?? string.Empty,
                DateRequested = hmacPartial.DateRequested,
                Nonce = hmacPartial.Nonce,
                HeaderValues = []
            };
        }
        else if (headers.TryParseHeaders(Options.Scheme, out var headerValues))
        {
            value = new Hmac
            {
                Policy = hmacPartial.Policy,
                Scheme = hmacPartial.Scheme,
                Signature = signature ?? string.Empty,
                DateRequested = hmacPartial.DateRequested,
                Nonce = hmacPartial.Nonce,
                HeaderValues = headerValues.ToArray()
            };
        }
        else
        {
            value = null;
        }

        return value is not null;
    }
}
