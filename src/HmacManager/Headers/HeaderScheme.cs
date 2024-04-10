namespace HmacManager.Headers;

/// <summary>
/// A class representing a <c>HeaderScheme</c>..
/// </summary>
public class HeaderScheme
{
    protected HeaderCollection Headers = new();

    public HeaderScheme(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        Name = name;
    }

    internal readonly string Name;

    internal IReadOnlyCollection<Header> GetHeaders() => Headers.GetAll();

    /// <summary>
    /// Adds a specified header name to the <c>HeaderScheme</c>. This header
    /// will be required for authentication to succeed.
    /// </summary>
    /// <param name="name">The name of the header on the <c>HttpRequestMessage</c>.</param>
    public void AddHeader(string name) =>
        AddHeader(name, name);

    /// <summary>
    /// Adds a specified header name to the <c>HeaderScheme</c>. This header
    /// will be required for authentication to succeed.
    /// </summary>
    /// <param name="name">The name of the header on the HTTP request.</param>
    /// <param name="claimType">The name of the claim that the header value should be converted to.</param>
    public void AddHeader(string name, string claimType) =>
        Headers.Add(name, configureHeader =>
        {
            configureHeader.Name = name;
            configureHeader.ClaimType = claimType;
        });
}