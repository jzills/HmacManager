using HmacManagement.Caching;
using HmacManagement.Headers;

namespace HmacManagement.Policies;

public class HmacPolicy
{
    public KeyCredentials Keys { get; set; } = new();
    public Algorithms Algorithms { get; set; } = new();
    public Nonce Nonce { get; set; } = new();

    protected readonly Dictionary<string, HeaderScheme> HeaderSchemes = new();

    public IReadOnlyDictionary<string, HeaderScheme> GetHeaderSchemes() => HeaderSchemes.AsReadOnly();

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