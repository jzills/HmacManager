namespace HmacManager.Headers;

public class HeaderScheme
{
    protected HeaderCollection Headers = new();

    public HeaderScheme(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
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