using HmacManager.Caching;
using HmacManager.Components;
using HmacManager.Headers;

namespace HmacManager.Policies;

/// <summary>
/// A class representing a builder for a HmacPolicy.
/// </summary>
public class HmacPolicyBuilder
{
    protected readonly KeyCredentials Keys = new();
    protected readonly Algorithms Algorithms = new();
    protected readonly Nonce Nonce = new();
    protected readonly HeaderSchemeCollection HeaderSchemes = new();

    /// <summary>
    /// Uses the specified guid as the public key for this policy.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>An instance of HmacPolicyBuilder.</returns>
    public HmacPolicyBuilder UsePublicKey(Guid key)
    {
        Keys.PublicKey = key;
        return this;
    }

    /// <summary>
    /// Uses the specified base64 string as the private key for this policy.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>An instance of HmacPolicyBuilder.</returns>
    public HmacPolicyBuilder UsePrivateKey(string key)
    {
        Keys.PrivateKey = key;
        return this;
    }

    /// <summary>
    /// Uses the specified algorithm to hash request content as part of the signature.
    /// </summary>
    /// <param name="hashAlgorithm"></param>
    /// <returns>An instance of HmacPolicyBuilder.</returns>
    public HmacPolicyBuilder UseContentHashAlgorithm(ContentHashAlgorithm hashAlgorithm)
    {
        Algorithms.ContentHashAlgorithm = hashAlgorithm;
        return this;
    }

    /// <summary>
    /// Uses the specified algorithm to sign the mac.
    /// </summary>
    /// <param name="hashAlgorithm"></param>
    /// <returns>An instance of HmacPolicyBuilder.</returns>
    public HmacPolicyBuilder UseSigningHashAlgorithm(SigningHashAlgorithm hashAlgorithm)
    {
        Algorithms.SigningHashAlgorithm = hashAlgorithm;
        return this;
    }

    /// <summary>
    /// Sets the maximum age on an http request and the ttl on the memory cache entry for the request nonce.
    /// </summary>
    /// <param name="maxAge"></param>
    /// <returns>An instance of HmacPolicyBuilder.</returns>
    public HmacPolicyBuilder UseMemoryCache(TimeSpan maxAge)
    {
        Nonce.CacheName = "Memory";
        Nonce.MaxAge = maxAge;
        return this;
    }

    /// <summary>
    /// Sets the maximum age on an http request and the ttl on the distributed cache entry for the request nonce.
    /// </summary>
    /// <param name="maxAge"></param>
    /// <returns>An instance of HmacPolicyBuilder.</returns>
    public HmacPolicyBuilder UseDistributedCache(TimeSpan maxAge)
    {
        Nonce.CacheName = "Distributed";
        Nonce.MaxAge = maxAge;
        return this;
    }

    /// <summary>
    /// Adds a specified scheme to the HeaderSchemes collection.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="configureScheme"></param>
    /// <returns>An instance of HmacPolicyBuilder.</returns>
    public HmacPolicyBuilder AddScheme(string name, Action<HeaderScheme> configureScheme)
    {
        HeaderSchemes.Add(name, configureScheme);   
        return this;
    }

    /// <summary>
    /// Builds an instance of HmacPolicy.
    /// </summary>
    /// <returns>An instance of the configured HmacPolicy.</returns>
    public HmacPolicy Build()
    {
        if (string.IsNullOrWhiteSpace(Nonce.CacheName))
        {
            Nonce.CacheName = "Memory";
        }

        return new HmacPolicy
        {
            Algorithms = Algorithms,
            Keys = Keys,
            Nonce = Nonce,
            HeaderSchemes = HeaderSchemes
        };
    }
}