using HmacManager.Caching;
using HmacManager.Components;
using HmacManager.Headers;

namespace HmacManager.Policies;

/// <summary>
/// A class representing a <c>HmacPolicyBuilder</c> for creating a <c>HmacPolicy</c>.
/// </summary>
public class HmacPolicyBuilder
{
    protected readonly KeyCredentials Keys = new();
    protected readonly Algorithms Algorithms = new();
    protected readonly Nonce Nonce = new();
    protected readonly HeaderSchemeCollection HeaderSchemes = new();  

    /// <summary>
    /// Uses the specified <c>Guid</c> as the public key for this <c>HmacPolicy</c>.
    /// </summary>
    /// <param name="key">The public key.</param>
    /// <returns>An <c>HmacPolicyBuilder</c> that can be used to further configure the policy.</returns>
    public HmacPolicyBuilder UsePublicKey(Guid key)
    {
        Keys.PublicKey = key;
        return this;
    }

    /// <summary>
    /// Uses the specified base64 encoded <c>string</c> as the private key for this <c>HmacPolicy</c>.
    /// </summary>
    /// <param name="key">The private key.</param>
    /// <returns>An <c>HmacPolicyBuilder</c> that can be used to further configure the policy.</returns>
    public HmacPolicyBuilder UsePrivateKey(string key)
    {
        Keys.PrivateKey = key;
        return this;
    }

    /// <summary>
    /// Uses the specified <c>ContentHashAlgorithm</c> to hash the <c>HttpRequestMessage</c> content as part of the signature.
    /// </summary>
    /// <param name="hashAlgorithm">The <c>ContentHashAlgorithm</c>.</param>
    /// <returns>An <c>HmacPolicyBuilder</c> that can be used to further configure the policy.</returns>
    public HmacPolicyBuilder UseContentHashAlgorithm(ContentHashAlgorithm hashAlgorithm)
    {
        Algorithms.ContentHashAlgorithm = hashAlgorithm;
        return this;
    }

    /// <summary>
    /// Uses the specified <c>SigningHashAlgorithm</c> to sign the content for authentication.
    /// </summary>
    /// <param name="hashAlgorithm">The <c>SigningHashAlgorithm</c>.</param>
    /// <returns>An <c>HmacPolicyBuilder</c> that can be used to further configure the policy.</returns>
    public HmacPolicyBuilder UseSigningHashAlgorithm(SigningHashAlgorithm hashAlgorithm)
    {
        Algorithms.SigningHashAlgorithm = hashAlgorithm;
        return this;
    }

    /// <summary>
    /// Sets the maximum age on an <c>HttpRequestMessage</c> and the TTL for nonce cache entries.
    /// </summary>
    /// <param name="maxAge">The <c>TimeSpan</c> representing the max age of a request.</param>
    /// <returns>An <c>HmacPolicyBuilder</c> that can be used to further configure the policy.</returns>
    public HmacPolicyBuilder UseMemoryCache(TimeSpan maxAge)
    {
        Nonce.CacheType = NonceCacheType.Memory;
        Nonce.MaxAge = maxAge;
        return this;
    }

    /// <summary>
    /// Sets the maximum age on an <c>HttpRequestMessage</c> and the TTL for nonce cache entries.
    /// </summary>
    /// <param name="maxAge">The <c>TimeSpan</c> representing the max age of a request.</param>
    /// <returns>An <c>HmacPolicyBuilder</c> that can be used to further configure the policy.</returns>
    public HmacPolicyBuilder UseDistributedCache(TimeSpan maxAge)
    {
        Nonce.CacheType = NonceCacheType.Distributed;
        Nonce.MaxAge = maxAge;
        return this;
    }

    /// <summary>
    /// Adds a specified scheme to the <c>HeaderSchemeCollection</c>. This can later be used
    /// to authenticate signatures.
    /// </summary>
    /// <param name="name">The name of the <c>HeaderScheme</c>.</param>
    /// <param name="configureScheme">The configuration action for <c>HeaderScheme</c>.</param>
    /// <returns>An <c>HmacPolicyBuilder</c> that can be used to further configure the policy.</returns>
    public HmacPolicyBuilder AddScheme(string name, Action<HeaderScheme> configureScheme)
    {
        HeaderSchemes.Add(name, configureScheme);   
        return this;
    }

    /// <summary>
    /// Builds an instance of the configured <c>HmacPolicy</c>.
    /// </summary>
    /// <returns>An <c>HmacPolicyBuilder</c> that can be used to further configure the policy.</returns>
    public HmacPolicy Build(string? name = null) => 
        new HmacPolicy(name)
        {
            Algorithms = Algorithms,
            Keys = Keys,
            Nonce = Nonce,
            HeaderSchemes = HeaderSchemes
        };
}