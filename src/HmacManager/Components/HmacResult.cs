namespace HmacManager.Components;

/// <summary>
/// A class representing a single, generated Hmac.
/// </summary>
public class HmacResult
{
    /// <summary>
    /// Represents the policy used by HmacManager for signing and verification.
    /// </summary>
    /// <value>The name of the policy.</value>
    public required string Policy { get; init; }
    /// <summary>
    /// Represents the header scheme used by HmacManager for signing and verification.
    /// </summary>
    /// <value>The name of the header scheme.</value>
    public required string HeaderScheme { get; init; }
    public Hmac? Hmac { get; init; } = default;
    /// <summary>
    /// Represents the success of the authentication operation, i.e. signing or verification.
    /// </summary>
    /// <value>True if the HmacResult generation is successful, otherwise false.</value>
    public bool IsSuccess { get; init; } = false;
    /// <summary>
    /// Represents the date the HmacResult is generated.
    /// </summary>
    /// <value>A utc date and time.</value>
    public DateTime DateGenerated { get; } = DateTime.UtcNow;
}