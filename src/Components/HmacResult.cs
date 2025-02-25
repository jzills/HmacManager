namespace HmacManager.Components;

/// <summary>
/// A class representing a <c>HmacResult</c>.
/// </summary>
public sealed class HmacResult
{
    /// <summary>
    /// Represents the resulting <c>Hmac</c>.
    /// </summary>
    /// <value>The <c>Hmac</c>.</value>
    public readonly Hmac? Hmac;

    /// <summary>
    /// Represents the success of the authentication operation, i.e. signing or verification.
    /// </summary>
    /// <value>True if the <c>HmacResult</c> generation is successful, otherwise false.</value>
    public readonly bool IsSuccess;

    /// <summary>
    /// Represents the date and time the <c>HmacResult</c> is generated.
    /// </summary>
    /// <value>A UTC date and time.</value>
    public readonly DateTime DateGenerated = DateTime.UtcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="HmacResult"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="hmac">The HMAC (Hash-based Message Authentication Code) associated with the result, or null if not available.</param>
    internal HmacResult(bool isSuccess, Hmac? hmac)
    {
        IsSuccess = isSuccess;
        Hmac = hmac;
    }
}