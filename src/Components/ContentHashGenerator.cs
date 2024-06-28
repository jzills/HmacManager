using System.Security.Cryptography;

namespace HmacManager.Components;

/// <summary>
/// A class representing a <c>ContentHashGenerator</c>.
/// </summary>
public class ContentHashGenerator : IHashGenerator
{
    private readonly HmacProviderOptions _options;

    /// <summary>
    /// Creates a <c>ContentHashGenerator</c> object.
    /// </summary>
    /// <param name="options"><c>HmacProviderOptions</c></param>
    /// <returns>A <c>ContentHashGenerator</c> object.</returns>
    public ContentHashGenerator(HmacProviderOptions options) => _options = options;

    /// <inheritdoc/>
    public Task<string> HashAsync(string content)
    {
        using HashAlgorithm hashAlgorithm = _options.Algorithms.ContentHashAlgorithm switch
        {
            ContentHashAlgorithm.SHA1   => SHA1  .Create(),
            ContentHashAlgorithm.SHA256 => SHA256.Create(),
            ContentHashAlgorithm.SHA512 => SHA512.Create(),
            _                           => SHA256.Create()
        };

        return new HashExecutor(hashAlgorithm).ExecuteAsync(content);
    }
}