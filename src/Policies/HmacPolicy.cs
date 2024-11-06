using System.Text.Json.Serialization;
using HmacManager.Caching;
using HmacManager.Components;
using HmacManager.Headers;

namespace HmacManager.Policies;

/// <summary>
/// A class representing a <c>HmacPolicy</c>.
/// </summary>
public class HmacPolicy
{
    /// <summary>
    /// The name of the policy.
    /// </summary>
    [JsonInclude]
    public string? Name { get; init; }

    /// <summary>
    /// Creates an instance of <c>HmacPolicy</c>.
    /// </summary> 
    internal HmacPolicy() { }

    /// <summary>
    /// Creates an <c>HmacPolicy</c> from a specified name.
    /// </summary>
    /// <param name="name">The name of the <c>HmacPolicy</c>.</param>
    public HmacPolicy(string? name) => Name = name;

    /// <summary>
    /// The <c>KeyCredentials</c> used to sign and verify authentication codes.
    /// This includes both <c>PublicKey</c> and <c>PrivateKey</c>.
    /// </summary>
    public KeyCredentials Keys { get; set; } = new();

    /// <summary>
    /// The <c>Algorithms</c> used to compute hash values.
    /// This includes both <c>ContentHashAlgorithm</c> and <c>SignatureContentAlgorithm</c>.
    /// </summary>
    public Algorithms Algorithms { get; set; } = new();
    
    /// <summary>
    /// A <c>Nonce</c> object containing the cache type and max age.
    /// </summary>
    public Nonce Nonce { get; set; } = new();

    /// <summary>
    /// A collection of <c>HeaderScheme</c> objects for this policy.
    /// </summary>
    public HeaderSchemeCollection HeaderSchemes { get; set; } = new();

    /// <summary>
    /// The <c>SigningContentBuilder</c> used to construct signing content.
    /// </summary>
    public SigningContentBuilder SigningContentBuilder { get; set; } = new SigningContentBuilderValidated();
}