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
    protected HashSet<Header> Headers = new();

    public HeaderScheme(string name)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));
        
        Name = name;
    }

    public readonly string Name;

    public IReadOnlyCollection<Header> GetRequiredHeaders() => 
        Headers.ToList().AsReadOnly();

    public void AddRequiredHeader(string name) =>
        AddRequiredHeader(name, name);

    public void AddRequiredHeader(string name, string claimType)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));
        ArgumentNullException.ThrowIfNullOrEmpty(claimType, nameof(claimType));

        Headers.Add(new Header(name, claimType));
    }
}