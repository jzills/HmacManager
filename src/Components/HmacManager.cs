using HmacManager.Caching;
using HmacManager.Caching.Extensions;
using HmacManager.Extensions;

namespace HmacManager.Components;

/// <summary>
/// A class representing a <c>HmacManager</c>.
/// </summary>
public class HmacManager : IHmacManager
{
    /// <summary>
    /// An instance of <c>HmacManagerOptions</c>.
    /// </summary>
    protected readonly HmacManagerOptions Options;

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

    /// <Optionsnheritdoc/>
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
                if (hmacVerification?.Signature == incomingHmac.Signature)
                {
                    return ResultFactory.Success(incomingHmac);
                }
            }
        }
        
        return ResultFactory.Failure();
    }

    /// <inheritdoc/>
    public async Task<HmacResult> SignAsync(HttpRequestMessage request)
    {
        var hmac = await Factory.CreateAsync(request, Options.HeaderScheme);
        if (hmac is not null)
        {
            request.Headers.AddRequiredHeaders(hmac, 
                Options.Policy, 
                Options.HeaderScheme?.Name
            );
            
            return ResultFactory.Success(hmac);
        }
        else
        {
            return ResultFactory.Failure();
        }
    }
}
