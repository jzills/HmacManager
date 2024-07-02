namespace HmacManager.Components;

/// <summary>
/// A class representing a <c>HmacResult</c>.
/// </summary>
public class HmacResult
{
    /// <summary>
    /// Represents the policy used by HmacManager for signing and verification.
    /// </summary>
    /// <value>The name of the policy.</value>
    public readonly string Policy;

    /// <summary>
    /// Represents the name of the <c>HeaderScheme</c> used by <c>HmacManager</c> for signing and verification.
    /// </summary>
    /// <value>The name of the <c>HeaderScheme</c>.</value>
    public readonly string HeaderScheme;

    /// <summary>
    /// Represents the resulting <c>Hmac</c>.
    /// </summary>
    /// <value>The <c>Hmac</c>.</value>
    public Hmac? Hmac { get; init; } = default;

    /// <summary>
    /// Represents the success of the authentication operation, i.e. signing or verification.
    /// </summary>
    /// <value>True if the <c>HmacResult</c> generation is successful, otherwise false.</value>
    public bool IsSuccess { get; init; } = false;

    /// <summary>
    /// Represents the date and time the <c>HmacResult</c> is generated.
    /// </summary>
    /// <value>A UTC date and time.</value>
    public DateTime DateGenerated { get; } = DateTime.UtcNow;

    internal HmacResult(string policy, string headerScheme)
    {
        Policy = policy;
        HeaderScheme = headerScheme;
    }
}