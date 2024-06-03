using HmacManager.Caching;
using HmacManager.Common;
using HmacManager.Common.Extensions;
using HmacManager.Headers;
using HmacManager.Policies;

namespace HmacManager.Components;

/// <summary>
/// A class representing a <c>HmacManagerFactory</c>.
/// </summary>
public class HmacManagerFactory : IHmacManagerFactory
{
    /// <summary>
    /// An <c>IComponentCollection</c> of <c>HmacPolicy</c> objects.
    /// </summary>
    protected readonly IComponentCollection<HmacPolicy> Policies;

    /// <summary>
    /// An <c>IComponentCollection</c> of <c>INonceCache</c> objects.
    /// </summary>
    protected readonly IComponentCollection<INonceCache> Caches;

    /// <summary>
    /// Creates a <c>HmacManagerFactory</c> object.
    /// </summary>
    /// <param name="policies"><c>HmacManagerOptions</c></param>
    /// <param name="caches"><c>IHmacProvider</c></param>
    /// <returns>A <c>HmacManagerFactory</c> object.</returns>
    public HmacManagerFactory(
        IComponentCollection<HmacPolicy> policies,
        IComponentCollection<INonceCache> caches
    )
    {
        Policies = policies;
        Caches = caches;
    }

    /// <inheritdoc/>
    public IHmacManager? Create(string policy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(policy, nameof(policy));
        
        if (TryGetPolicyCache(policy, out var options, out var cache))
        {
            return CreateManager(options, cache);
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public IHmacManager? Create(string policy, string scheme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(policy, nameof(policy));
        ArgumentException.ThrowIfNullOrWhiteSpace(scheme, nameof(scheme));

        if (TryGetPolicyCache(policy, out var options, out var cache))
        {
            return CreateManager(options, cache, scheme);
        }
        else
        {
            return null;
        }
    }

    private HmacManager CreateManager(HmacPolicy options, INonceCache cache) =>
        new HmacManager(
            CreateOptions(options.Name!, options.Nonce.MaxAgeInSeconds),
            CreateProvider(options.Keys, options.Algorithms),
            cache
        );

    private HmacManager CreateManager(HmacPolicy options, INonceCache cache, string scheme) =>
        new HmacManager(
            CreateOptions(
                options.Name!, 
                options.Nonce.MaxAgeInSeconds, 
                options.HeaderSchemes.Get(scheme)
            ),
            CreateProvider(options.Keys, options.Algorithms),
            cache
        );

    private HmacProvider CreateProvider(KeyCredentials keys, Algorithms algorithms)
    {
        var options = new HmacProviderOptions { Keys = keys, Algorithms = algorithms};
        return new HmacProvider(
            options,
            new ContentHashGenerator(options),
            new SignatureHashGenerator(options)
        );
    }

    private HmacManagerOptions CreateOptions(
        string policy, 
        int maxAgeInSeconds, 
        HeaderScheme? scheme = null
    ) => new HmacManagerOptions(policy) { MaxAgeInSeconds = maxAgeInSeconds, HeaderScheme = scheme };

    private bool TryGetPolicyCache(string policy, out HmacPolicy options, out INonceCache cache) => 
        Policies.TryGetValue(policy, out options) &&
          Caches.TryGetValue(Enum.GetName(options.Nonce.CacheType)!, out cache) || (cache = default!) != default!;
}