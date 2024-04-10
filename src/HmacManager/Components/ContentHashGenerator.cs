using System.Security.Cryptography;

namespace HmacManager.Components;

public class ContentHashGenerator : IHashGenerator
{
    private readonly HmacProviderOptions _options;

    public ContentHashGenerator(HmacProviderOptions options) => _options = options;

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