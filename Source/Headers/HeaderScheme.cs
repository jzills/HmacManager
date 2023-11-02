using HmacManagement.Policies;

namespace HmacManagement.Headers;

public class Policy
{
    public Policy(string name, HeaderScheme scheme)
    {
        Name = name;
        Scheme = scheme;
    }

    public readonly string Name;
    public readonly HeaderScheme Scheme;
}

public class HeaderScheme
{
    protected HeaderCollection Headers = new();

    public HeaderScheme(string name)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));
        Name = name;
    }

    public readonly string Name;

    public IReadOnlyCollection<Header> GetHeaders() => Headers.GetAll();

    public void AddHeader(string name) =>
        AddHeader(name, name);

    public void AddHeader(string name, string claimType) =>
        Headers.Add(name, configureHeader =>
        {
            configureHeader.Name = name;
            configureHeader.ClaimType = claimType;
        });
}

public class HeaderCollection : ComponentCollection<Header>, IConfigurator<Header>
{
    // TODO: Add validator
    
    public void Add(string name, Action<Header> configureHeader)
    {
        var header = new Header();
        configureHeader.Invoke(header);
    }
}