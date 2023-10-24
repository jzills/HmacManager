namespace HmacManagement.Headers;

public class HeaderSchemeCollection
{
    protected IDictionary<string, HeaderScheme> Schemes = new Dictionary<string, HeaderScheme>();
    
    public IReadOnlyDictionary<string, HeaderScheme> GetHeaderSchemes() => Schemes.AsReadOnly();

    public HeaderScheme? GetHeaderScheme(string name)
    {
        if (Schemes.ContainsKey(name))
        {
            return Schemes[name];
        }
        else
        {
            return default;
        }
    }

    public void AddHeaderScheme(string name, Action<HeaderScheme> configureScheme)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

        var scheme = new HeaderScheme(name);
        configureScheme.Invoke(scheme);

        ArgumentNullException.ThrowIfNull(scheme);

        Schemes.Add(scheme.Name, scheme);
    }
}