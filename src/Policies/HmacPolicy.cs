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
    ///     <para>
    ///         <list>
    ///             <item>See <see cref="Policies.Algorithms">here</see> for <c>Algorithm</c> definition.</item>
    ///             <item>See <see cref="ContentHashAlgorithm">here</see> for <c>ContentHashAlgorithm</c> definition.</item>
    ///             <item>See <see cref="SigningHashAlgorithm">here</see> for <c>SigningHashAlgorithm</c> definition.</item>
    ///         </list>
    ///     </para>
    /// </summary>
    public Algorithms Algorithms { get; set; } = new();
    public Nonce Nonce { get; set; } = new();
    public HeaderSchemeCollection HeaderSchemes { get; set; } = new();
}