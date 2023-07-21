using System.Collections.ObjectModel;

namespace HmacManager.Headers;

public class HeaderSchemeCollection
{
    private Dictionary<string, List<string>> _headerSchemes = new();
    public void Add(string headerScheme, Action<HeaderSchemeOptions> configureScheme)
    {
        var options = new HeaderSchemeOptions();
        configureScheme.Invoke(options);

        if (_headerSchemes.TryGetValue(headerScheme, out var headers))
        {
            headers.AddRange(options.Headers);
        }
        else
        {
            _headerSchemes[headerScheme] = new List<string>(options.Headers);
        }
    }
    public List<string> GetScheme(string headerScheme) =>
        _headerSchemes.TryGetValue(headerScheme, out var scheme) ?
            scheme : 
            new List<string>(0);
}

public class HeaderSchemeOptions
{
    private HashSet<string> _headers = new();
    public ReadOnlyCollection<string> Headers => _headers.ToList().AsReadOnly();
    public void Add(string name) => _headers.Add(name);
}