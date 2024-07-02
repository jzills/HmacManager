using System.Security.Cryptography;

namespace HmacManager.Components;

/// <summary>
/// A class representing a <c>ContentHashGenerator</c>.
/// </summary>
public class ContentHashGenerator : IHashGenerator
{
    private readonly ContentHashAlgorithm _contentHashAlgorithm;

    /// <summary>
    /// Creates a <c>ContentHashGenerator</c> object.
    /// </summary>
    /// <param name="contentHashAlgorithm"><c>ContentHashAlgorithm</c></param>
    /// <returns>A <c>ContentHashGenerator</c> object.</returns>
    public ContentHashGenerator(ContentHashAlgorithm contentHashAlgorithm) => _contentHashAlgorithm = contentHashAlgorithm;

    /// <inheritdoc/>
    public Task<string> HashAsync(string content)
    {
        using HashAlgorithm hashAlgorithm = _contentHashAlgorithm switch
        {
            ContentHashAlgorithm.SHA1   => SHA1  .Create(),
            ContentHashAlgorithm.SHA256 => SHA256.Create(),
            ContentHashAlgorithm.SHA512 => SHA512.Create(),
            _                           => SHA256.Create()
        };

        return new HashExecutor(hashAlgorithm).ExecuteAsync(content);
    }
}