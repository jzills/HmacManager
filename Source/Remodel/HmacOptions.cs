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
    public void AddDefaultKeys(string publicKey, string privateKey)
    {

    }
    public void AddHeaderScheme(string name, Action<HeaderScheme> configureScheme)
    {

    }
}