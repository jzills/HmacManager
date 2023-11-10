using HmacManagement.Caching;
using HmacManagement.Components;
using HmacManagement.Headers;

namespace HmacManagement.Policies;

public class HmacPolicyBuilder
{
    protected readonly KeyCredentials Keys = new();
    protected readonly Algorithms Algorithms = new();
    protected readonly Nonce Nonce = new();
    protected readonly HeaderSchemeCollection HeaderSchemes = new();

    public HmacPolicyBuilder UsePublicKey(Guid key)
    {
        Keys.PublicKey = key;
        return this;
    }

    public HmacPolicyBuilder UsePrivateKey(string key)
    {
        Keys.PrivateKey = key;
        return this;
    }

    public HmacPolicyBuilder UseContentHashAlgorithm(ContentHashAlgorithm hashAlgorithm)
    {
        Algorithms.ContentHashAlgorithm = hashAlgorithm;
        return this;
    }

    public HmacPolicyBuilder UseSigningHashAlgorithm(SigningHashAlgorithm hashAlgorithm)
    {
        Algorithms.SigningHashAlgorithm = hashAlgorithm;
        return this;
    }

    public HmacPolicyBuilder UseInMemoryCache(TimeSpan maxAge)
    {
        Nonce.CacheName = "InMemory";
        Nonce.MaxAge = maxAge;
        return this;
    }

    public HmacPolicyBuilder UseDistributedCache(TimeSpan maxAge)
    {
        Nonce.CacheName = "Distributed";
        Nonce.MaxAge = maxAge;
        return this;
    }

    public HmacPolicyBuilder AddScheme(string name, Action<HeaderScheme> configureScheme)
    {
        HeaderSchemes.Add(name, configureScheme);   
        return this;
    }

    public HmacPolicy Build() =>
        new HmacPolicy
        {
            Algorithms = Algorithms,
            Keys = Keys,
            Nonce = Nonce,
            HeaderSchemes = HeaderSchemes
        };
}