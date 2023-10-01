using HmacManagement.Caching;
using HmacManagement.Components;

namespace HmacManagement.Remodel;

public class HeaderScheme
{
    public void AddRequiredHeader(string name)
    {

    }

    public void AddRequiredHeader(string header, string claimType)
    {

    }
}

public class HmacOptions
{
    public KeyCredentials Keys { get; set; }
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public Algorithms Algorithms { get; set; }
    public NonceCacheType NonceCacheType { get; set; } = NonceCacheType.Memory;
    public void AddDefaultKeys(string publicKey, string privateKey)
    {

    }
    public void AddHeaderScheme(string name, Action<HeaderScheme> configureScheme)
    {

    }
}