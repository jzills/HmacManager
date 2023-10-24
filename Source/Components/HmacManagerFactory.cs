using HmacManagement.Caching;
using HmacManagement.Policies;

namespace HmacManagement.Components;

public class HmacManagerFactory : IHmacManagerFactory
{
    protected IHmacPolicyCollection Policies;
    protected INonceCacheCollection Caches;

    public HmacManagerFactory(
        IHmacPolicyCollection policies,
        INonceCacheCollection caches
    )
    {
        Policies = policies;
        Caches = caches;
    }

    public IHmacManager Create() => Create("Default");

    public IHmacManager Create(string policy)
    {
        var policyOptions = Policies.GetPolicy(policy);
        if (policyOptions is null)
        {
            throw new Exception($"There are no \"HmacPolicy\" registered for the policy \"{policy}\".");
        }

        var nonceCache = Caches.GetCache(policyOptions.Nonce.CacheType);
        if (nonceCache is null)
        {
            throw new Exception($"There is no cache registered for the cache type \"{policyOptions.Nonce.CacheType}\".");
        }

        return new HmacManager(
            new HmacManagerOptions { MaxAge = policyOptions.Nonce.MaxAge },
            nonceCache,
            new HmacProvider(
                new HmacProviderOptions 
                { 
                    Keys = policyOptions.Keys, 
                    Algorithms = policyOptions.Algorithms 
                }
            )
        );
    }

    public IHmacManager Create(string policy, string scheme)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(policy, nameof(policy));
        ArgumentNullException.ThrowIfNullOrEmpty(scheme, nameof(scheme));

        var policyOptions = Policies.GetPolicy(policy);
        if (policyOptions is null)
        {
            throw new Exception($"There are no \"HmacPolicy\" registered for the policy \"{policy}\".");
        }

        var nonceCache = Caches.GetCache(policyOptions.Nonce.CacheType);
        if (nonceCache is null)
        {
            throw new Exception($"There is no cache registered for the cache type \"{policyOptions.Nonce.CacheType}\".");
        }

        return new HmacManager(
            new HmacManagerOptions 
            { 
                MaxAge = policyOptions.Nonce.MaxAge,
                HeaderScheme = policyOptions.HeaderSchemes.GetHeaderScheme(scheme)
            },
            nonceCache,
            new HmacProvider(
                new HmacProviderOptions 
                { 
                    Keys = policyOptions.Keys, 
                    Algorithms = policyOptions.Algorithms 
                }
            )
        );
    }
}