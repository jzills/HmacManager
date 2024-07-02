using System.Security.Cryptography;

namespace HmacManager.Components;

/// <summary>
/// A class representing a <c>SignatureHashGenerator</c>.
/// </summary>
public class SignatureHashGenerator : IHashGenerator
{
    private readonly string? _privateKey;
    private readonly SigningHashAlgorithm _signingHashAlgorithm;

    /// <summary>
    /// Creates a <c>SignatureHashGenerator</c> object.
    /// </summary>
    /// <param name="privateKey"><c>string?</c></param>/// 
    /// <param name="signingHashAlgorithm"><c>SigningHashAlgorithm</c></param>
    /// <returns>A <c>SignatureHashGenerator</c> object.</returns>
    public SignatureHashGenerator(
        string? privateKey, 
        SigningHashAlgorithm signingHashAlgorithm
    ) => (_privateKey, _signingHashAlgorithm) = (privateKey, signingHashAlgorithm);

    /// <inheritdoc/>
    public Task<string> HashAsync(string content)
    {
        var keyBytes = Convert.FromBase64String(_privateKey);
        using HashAlgorithm hashAlgorithm = _signingHashAlgorithm switch
        {
            SigningHashAlgorithm.HMACSHA1   => new HMACSHA1  (keyBytes),
            SigningHashAlgorithm.HMACSHA256 => new HMACSHA256(keyBytes),
            SigningHashAlgorithm.HMACSHA512 => new HMACSHA512(keyBytes),
            _                               => new HMACSHA256(keyBytes)
        };

        return new HashExecutor(hashAlgorithm).ExecuteAsync(content);
    }
}