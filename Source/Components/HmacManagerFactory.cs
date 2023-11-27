using HmacManagement.Caching;
using HmacManagement.Common;
using HmacManagement.Mvc;
using HmacManagement.Policies;

namespace HmacManagement.Components;

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

    public IHmacManager Create() => Create(HmacAuthenticationDefaults.DefaultPolicy);

    public IHmacManager Create(string policy)
    {
        var policyOptions = Policies.Get(policy);
        if (policyOptions is null)
        {
            throw new Exception($"There are no \"HmacPolicy\" registered for the policy \"{policy}\".");
        }

        var nonceCache = Caches.Get(policyOptions.Nonce.CacheName);
        if (nonceCache is null)
        {
            throw new Exception($"There is no cache registered for the cache type \"{policyOptions.Nonce.CacheName}\".");
        }

        return new HmacManager(
            new HmacManagerOptions(policyOptions.Name) { MaxAge = policyOptions.Nonce.MaxAge },
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

        var policyOptions = Policies.Get(policy);
        if (policyOptions is null)
        {
            throw new Exception($"There are no \"HmacPolicy\" registered for the policy \"{policy}\".");
        }

        var nonceCache = Caches.Get(policyOptions.Nonce.CacheName);
        if (nonceCache is null)
        {
            throw new Exception($"There is no cache registered for the cache type \"{policyOptions.Nonce.CacheName}\".");
        }

        return new HmacManager(
            new HmacManagerOptions(policyOptions.Name) 
            { 
                MaxAge = policyOptions.Nonce.MaxAge,
                HeaderScheme = policyOptions.HeaderSchemes.Get(scheme)
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