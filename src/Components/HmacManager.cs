using HmacManager.Caching;
using HmacManager.Caching.Extensions;
using HmacManager.Extensions;

namespace HmacManager.Components;

/// <summary>
/// A class representing a <c>HmacManager</c>.
/// </summary>
public class HmacManager : IHmacManager
{
    /// <inheritdoc/>
    public HmacManagerOptions Options { get; }

    /// <summary>
    /// An implementation of <c>IHmacFactory</c> for creating <c>Hmac</c> objects. 
    /// </summary>
    protected readonly IHmacFactory Factory;

    /// <summary>
    /// An implementation of <c>IHmacResultFactory</c> for creating <c>HmacResult</c> objects. 
    /// </summary>
    protected readonly IHmacResultFactory ResultFactory;

    /// <summary>
    /// An implementation of <c>INonceCache</c> for storing nonce values.
    /// </summary>
    protected readonly INonceCache Cache;

    /// <summary>
    /// Creates a <c>HmacManager</c> object.
    /// </summary>
    /// <param name="options"><c>HmacManagerOptions</c></param> 
    /// <param name="factory"><c>IHmacFactory</c></param>
    /// <param name="resultFactory"><c>IHmacResultFactory</c></param>
    /// <param name="cache"><c>INonceCache</c></param>
    /// <returns>A <c>HmacManager</c> object.</returns>
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
        if (request.Headers.TryParseHmac(
                Options.HeaderScheme, 
                Options.MaxAgeInSeconds, 
                out var incomingHmac
        ))
        {
            if (await Cache.IsValidNonceAsync(
                    incomingHmac.Nonce, 
                    incomingHmac.DateRequested
            ))
            {   
                var hmacVerification = await Factory.CreateAsync(request, incomingHmac);
                if (hmacVerification is not null && hmacVerification.IsVerified(incomingHmac))
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
        var hmac = await Factory.CreateAsync(request, Options.Policy, Options.HeaderScheme);
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
}
