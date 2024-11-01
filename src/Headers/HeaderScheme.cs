namespace HmacManager.Headers;

/// <summary>
/// A class representing a <c>HeaderScheme</c>.
/// </summary>
public class HeaderScheme
{
    private HeaderCollection _headers = new();

    /// <summary>
    /// Creates a <c>HeaderScheme</c> object.
    /// </summary>
    /// <param name="name"><c>string</c></param>
    /// <returns>A <c>HeaderScheme</c> object.</returns>
    public HeaderScheme(string? name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        Name = name;
    }

    internal string Name { get; init; }

    internal IReadOnlyCollection<Header> Headers => _headers.GetAll();

    /// <summary>
    /// Adds a specified header name to the <c>HeaderScheme</c>. This header
    /// will be required for authentication to succeed.
    /// </summary>
    /// <param name="name">The name of the header on the <c>HttpRequestMessage</c>.</param>
    public void AddHeader(string name) => AddHeader(name, name);

    /// <summary>
    /// Adds a specified header name to the <c>HeaderScheme</c>. This header
    /// will be required for authentication to succeed.
    /// </summary>
    /// <param name="name">The name of the header on the HTTP request.</param>
    /// <param name="claimType">The name of the claim that the header value should be converted to.</param>
    public void AddHeader(string name, string claimType) => _headers.Add(new Header(name, claimType));
}