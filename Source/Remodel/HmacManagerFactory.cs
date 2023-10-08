using HmacManagement.Caching;
using HmacManagement.Components;

namespace HmacManagement.Remodel;

public interface INonceCacheProvider
{
    INonceCache Get(NonceCacheType cacheType);
}

public interface IHmacManagerFactory
{
    IHmacManager Create(string policy);
    IHmacManager Create(string policy, string headerScheme);
}

public class HmacManagerFactory : IHmacManagerFactory
{
    protected IHmacPolicyProvider PolicyProvider;
    protected INonceCacheProvider NonceCacheProvider;

    public HmacManagerFactory(
        IHmacPolicyProvider policyProvider,
        INonceCacheProvider nonceCacheProvider
    )
    {
        PolicyProvider = policyProvider;
        NonceCacheProvider = nonceCacheProvider;
    }

    public IHmacManager Create()
    {
        // Get default
        throw new NotImplementedException();
    }

    public IHmacManager Create(string policy)
    {
        var policyOptions = PolicyProvider.Get(policy);
        if (policyOptions is null)
        {
            throw new Exception($"There are no \"HmacPolicy\" registered for the policy \"{policy}\".");
        }

        return new HmacManager(
            new HmacManagerOptions { MaxAge = policyOptions.Nonce.MaxAge },
            NonceCacheProvider.Get(policyOptions.Nonce.CacheType),
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

        var policyOptions = PolicyProvider.Get(policy);
        if (policyOptions is null)
        {
            throw new Exception($"There are no \"HmacPolicy\" registered for the policy \"{policy}\".");
        }

        var nonceCache = NonceCacheProvider.Get(policyOptions.Nonce.CacheType);
        if (nonceCache is null)
        {
            throw new Exception($"There is no cache registered for the cache type \"{policyOptions.Nonce.CacheType}\".");
        }

        return new HmacManager(
            new HmacManagerOptions 
            { 
                MaxAge = policyOptions.Nonce.MaxAge,
                HeaderScheme = policyOptions.GetHeaderScheme(scheme)
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