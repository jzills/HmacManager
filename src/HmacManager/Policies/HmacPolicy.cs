using System.Text.Json.Serialization;
using HmacManager.Caching;
using HmacManager.Headers;

namespace HmacManager.Policies;

public class HmacPolicy
{
    /// <summary>
    /// The name of the policy.
    /// </summary>
    [JsonInclude]
    public readonly string? Name;
    internal HmacPolicy() { }

    /// <summary>
    /// Creates an <c>HmacPolicy</c> from a specified name.
    /// </summary>
    /// <param name="name">The name of the <c>HmacPolicy</c></param>
    public HmacPolicy(string? name) => Name = name;
    public KeyCredentials Keys { get; set; } = new();
    public Algorithms Algorithms { get; set; } = new();
    public Nonce Nonce { get; set; } = new();
    public HeaderSchemeCollection HeaderSchemes { get; set; } = new();
}