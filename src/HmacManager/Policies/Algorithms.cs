using HmacManager.Components;

namespace HmacManager.Policies;

/// <summary>
/// A class representing the <c>Algorithms</c> used for <c>Hmac</c> generation.
/// </summary>
public class Algorithms
{
    /// <summary>
    /// Represents the hash algorithm used when hashing request content, if present.
    /// </summary>
    /// <value>A ContentHashAlgorithm enum value. The default value is SHA256.</value>
    public ContentHashAlgorithm ContentHashAlgorithm { get; set; } = ContentHashAlgorithm.SHA256;
    
    /// <summary>
    /// Represents the hash algorithm used when signing the constructed mac.
    /// </summary>
    /// <value>A SigningHashAlgorithm enum value. The default value is HAMCSHA256.</value>
    public SigningHashAlgorithm SigningHashAlgorithm { get; set; } = SigningHashAlgorithm.HMACSHA256;
}