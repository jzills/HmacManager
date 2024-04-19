using HmacManager.Caching;
using HmacManager.Common;
using HmacManager.Common.Extensions;
using HmacManager.Headers;
using HmacManager.Policies;

namespace HmacManager.Components;

public class HmacManagerFactory : IHmacManagerFactory
{
    protected IComponentCollection<HmacPolicy> Policies;
    protected IComponentCollection<INonceCache> Caches;

    public HmacManagerFactory(
        IComponentCollection<HmacPolicy> policies,
        IComponentCollection<INonceCache> caches
    )
    {
        Policies = policies;
        Caches = caches;
    }

    public IHmacManager? Create(string policy)
    {
        ArgumentException.ThrowIfNullOrEmpty(policy, nameof(policy));
        
        if (TryGetPolicyCache(policy, out var options, out var cache))
        {
            return CreateManager(options, cache);
        }
        else
        {
            return null;
        }
    }

    public IHmacManager? Create(string policy, string scheme)
    {
        ArgumentException.ThrowIfNullOrEmpty(policy, nameof(policy));
        ArgumentException.ThrowIfNullOrEmpty(scheme, nameof(scheme));

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
            CreateOptions(options.Name, options.Nonce.MaxAge),
            CreateProvider(options.Keys, options.Algorithms),
            cache
        );

    private HmacManager CreateManager(HmacPolicy options, INonceCache cache, string scheme) =>
        new HmacManager(
            CreateOptions(
                options.Name, 
                options.Nonce.MaxAge, 
                options.HeaderSchemes.Get(scheme)
            ),
            CreateProvider(options.Keys, options.Algorithms),
            cache
        );

    private HmacProvider CreateProvider(KeyCredentials keys, Algorithms algorithms)
    {
        var options = new HmacProviderOptions { Keys = keys, Algorithms = algorithms};
        return new HmacProvider(
            new ContentHashGenerator(options),
            new SignatureHashGenerator(options)
        );
    }

    private HmacManagerOptions CreateOptions(
        string policy, 
        TimeSpan maxAge, 
        HeaderScheme? scheme = null
    ) => new HmacManagerOptions(policy) { MaxAge = maxAge, HeaderScheme = scheme };

    private bool TryGetPolicyCache(string policy, out HmacPolicy options, out INonceCache cache) => 
        Policies.TryGetValue(policy, out options) &&
          Caches.TryGetValue(options.Nonce.CacheName, out cache) || (cache = default!) != default!;
}