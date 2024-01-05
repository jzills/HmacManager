using HmacManager.Headers;

namespace HmacManager.Components;

/// <summary>
/// A class representing a single, generated Hmac.
/// </summary>
public class Hmac
{
    /// <summary>
    /// Represents the Hmac signature generated from concatenating the following values together with a semicolon:<br/>
    /// <list>
    ///     <item>
    ///         <term>Method</term>
    ///         <description>The http method of the request.</description>
    ///     </item>
    ///     <item>
    ///         <term>Path, Query and Authority</term>
    ///         <description>If available, otherwise the original string for the uri is used.</description>
    ///     </item>
    ///     <item>
    ///         <term>Date Requested</term>
    ///         <description>The date the request was sent.</description>
    ///     </item>
    ///     <item>
    ///         <term>Public Key</term>
    ///         <description>The public key for the configured HmacManager.</description>
    ///     </item>
    ///     <item>
    ///         <term>Content Hash</term>
    ///         <description>If request content exists, the hash is generated as part of the signing content, otherwise this is skipped.</description>
    ///     </item>
    ///     <item>
    ///         <term>Header Values</term>
    ///         <description>These are the values configured for a specific scheme.</description>
    ///     </item>
    ///     <item>
    ///         <term>Nonce</term>
    ///         <description>Generated automatically by the configured HmacManager.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <value>
    /// The complete, hashed signature added to the authorization header based on the above.
    /// </value>
    public string Signature { get; set; } = string.Empty;
    /// <summary>
    /// Represents the date a request is made.
    /// </summary>
    /// <value>Defaults to the current utc time.</value>
    public DateTimeOffset DateRequested { get; init; } = DateTimeOffset.UtcNow;
    /// <summary>
    /// Represents a nonce to prevent replay attacks.
    /// </summary>
    /// <value>Defaults to a new guid.</value>
    public Guid Nonce { get; init; } = Guid.NewGuid();
    /// <summary>
    /// Represents the request content hash based on the configured HmacManager.
    /// </summary>
    /// <value>The hashed request content, if exists, otherwise null.</value>
    public string? SigningContent { get; set; }
    /// <summary>
    /// Represents custom header values configured on a per scheme basis. If defined, these become part of the signature.
    /// </summary>
    /// <value>An array of header values, empty if there is no scheme defined.</value>
    public HeaderValue[] HeaderValues { get; set; } = new HeaderValue[0];
}
