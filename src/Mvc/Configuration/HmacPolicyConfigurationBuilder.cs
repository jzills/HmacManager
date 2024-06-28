using HmacManager.Caching;
using HmacManager.Policies;

namespace HmacManager.Mvc.Configuration;

internal class HmacPolicyConfigurationBuilder : HmacPolicyBuilder
{
    public HmacPolicyConfigurationBuilder(HmacPolicyConfigurationSection policy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(policy.Name, nameof(policy.Name));
        ArgumentNullException.ThrowIfNull(policy.Algorithms, nameof(policy.Algorithms));
        ArgumentNullException.ThrowIfNull(policy.Keys, nameof(policy.Keys));
        ArgumentNullException.ThrowIfNull(policy.Nonce, nameof(policy.Nonce));

        UsePublicKey(policy.Keys.PublicKey);
        UsePrivateKey(policy.Keys.PrivateKey);
        UseContentHashAlgorithm(policy.Algorithms.ContentHashAlgorithm);
        UseSigningHashAlgorithm(policy.Algorithms.SigningHashAlgorithm);

        switch (policy.Nonce.CacheType)
        {
            case NonceCacheType.Memory:
                UseMemoryCache(policy.Nonce.MaxAgeInSeconds);
                break;
            case NonceCacheType.Distributed:
                UseDistributedCache(policy.Nonce.MaxAgeInSeconds);
                break;
            default:
                throw new ArgumentException($"The specified \"CacheType\" of {Enum.GetName(policy.Nonce.CacheType)} is not supported.");
        }

        if (policy.HeaderSchemes?.Any() ?? false)
        {
            foreach (var headerScheme in policy.HeaderSchemes)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(headerScheme.Name, nameof(headerScheme.Name));
                
                if (headerScheme.Headers?.Count > 0)
                {
                    AddScheme(headerScheme.Name, scheme =>
                    {
                        foreach (var header in headerScheme?.Headers ?? [])
                        {
                            scheme.AddHeader(header.Name, header.ClaimType);
                        }
                    });
                }
            }
        }
    }
}