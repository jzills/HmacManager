using HmacManagement.Caching;

namespace HmacManagement.Remodel;

public class Nonce
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public NonceCacheType CacheType { get; set; } = NonceCacheType.Memory;
}

public class HmacOptions
{
    public KeyCredentials Keys { get; set; } = new();
    public Algorithms Algorithms { get; set; } = new();
    public Nonce Nonce { get; set; } = new();

    protected readonly Dictionary<string, HeaderScheme> HeaderSchemes = new();

    public HeaderScheme? GetHeaderScheme(string name)
    {
        if (HeaderSchemes.ContainsKey(name))
        {
            return HeaderSchemes[name];
        }
        else
        {
            return default;
        }
    }

    public void AddDefaultKeys(string publicKey, string privateKey)
    {

    }
    public void AddHeaderScheme(string name, Action<HeaderScheme> configureScheme)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

        var scheme = new HeaderScheme(name);
        configureScheme.Invoke(scheme);

        HeaderSchemes.Add(name, scheme);
    }
}