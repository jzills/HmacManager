using HmacManager.Caching;
using HmacManager.Common;
using HmacManager.Common.Extensions;
using HmacManager.Headers;
using HmacManager.Policies;
using HmacManager.Policies.Extensions;

namespace HmacManager.Components;

/// <summary>
/// A class representing a <c>HmacManagerFactory</c>.
/// </summary>
public class HmacManagerFactory : IHmacManagerFactory
{
    /// <summary>
    /// An <c>IComponentCollection</c> of <c>HmacPolicy</c> objects.
    /// </summary>
    protected readonly IHmacPolicyCollection Policies;

    /// <summary>
    /// An <c>IComponentCollection</c> of <c>INonceCache</c> objects.
    /// </summary>
    protected readonly IComponentCollection<INonceCache> Caches;

    /// <summary>
    /// Creates a <c>HmacManagerFactory</c> object.
    /// </summary>
    /// <param name="policies"><c>HmacManagerOptions</c></param>
    /// <param name="caches"><c>IHmacSignatureProvider</c></param>
    /// <returns>A <c>HmacManagerFactory</c> object.</returns>
    public HmacManagerFactory(
        IHmacPolicyCollection policies,
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
            CreateFactory(options.Keys, options.Algorithms, options.SigningContentBuilder),
            CreateResultFactory(options.Name!),
            cache
        );

    private HmacManager CreateManager(HmacPolicy options, INonceCache cache, string scheme) =>
        new HmacManager(
            CreateOptions(
                options.Name!, 
                options.Nonce.MaxAgeInSeconds, 
                options.HeaderSchemes.Get(scheme)
            ),
            CreateFactory(options.Keys, options.Algorithms, options.SigningContentBuilder),
            CreateResultFactory(options.Name!, scheme),
            cache
        );

    private HmacFactory CreateFactory(
        KeyCredentials keys, 
        Algorithms algorithms,
        SigningContentBuilder signingContentBuilder
    ) => new HmacFactory(CreateProvider(keys, algorithms, signingContentBuilder));

    private HmacResultFactory CreateResultFactory(
        string policy, 
        string? scheme = null
    ) => new HmacResultFactory(policy, scheme);

    private HmacSignatureProvider CreateProvider(KeyCredentials keys, Algorithms algorithms, SigningContentBuilder signingContentBuilder) =>
        new HmacSignatureProvider(new HmacSignatureProviderOptions 
        { 
            Keys = keys, 
            Algorithms = algorithms,
            ContentHashGenerator = CreateContentHashGenerator(algorithms.ContentHashAlgorithm),
            SignatureHashGenerator = CreateSignatureHashGenerator(keys.PrivateKey, algorithms.SigningHashAlgorithm),
            SigningContentBuilder = signingContentBuilder
        });

    private HmacManagerOptions CreateOptions(
        string policy, 
        int maxAgeInSeconds, 
        HeaderScheme? scheme = null
    ) => new HmacManagerOptions(policy) { MaxAgeInSeconds = maxAgeInSeconds, HeaderScheme = scheme };

    private ContentHashGenerator CreateContentHashGenerator(
        ContentHashAlgorithm contentHashAlgorithm
    ) => new ContentHashGenerator(contentHashAlgorithm);

    private SignatureHashGenerator CreateSignatureHashGenerator(
        string? privateKey, 
        SigningHashAlgorithm signingHashAlgorithm
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(privateKey, nameof(privateKey));

        return new SignatureHashGenerator(privateKey, signingHashAlgorithm);
    }

    private bool TryGetPolicyCache(string policy, out HmacPolicy options, out INonceCache cache) => 
        Policies.TryGetValue(policy, out options) &&
          Caches.TryGetValue(Enum.GetName(options.Nonce.CacheType)!, out cache) || 
            (cache = default!) != default!;
}