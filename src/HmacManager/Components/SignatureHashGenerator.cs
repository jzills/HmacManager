using System.Security.Cryptography;

namespace HmacManager.Components;

public class SignatureHashGenerator : IHashGenerator
{
    private readonly HmacProviderOptions _options;

    public SignatureHashGenerator(HmacProviderOptions options) => _options = options;

    public Task<string> HashAsync(string content)
    {
        var keyBytes = Convert.FromBase64String(_options.Keys.PrivateKey);
        using HashAlgorithm hashAlgorithm = _options.Algorithms.SigningHashAlgorithm switch
        {
            SigningHashAlgorithm.HMACSHA1   => new HMACSHA1  (keyBytes),
            SigningHashAlgorithm.HMACSHA256 => new HMACSHA256(keyBytes),
            SigningHashAlgorithm.HMACSHA512 => new HMACSHA512(keyBytes),
            _                               => new HMACSHA256(keyBytes)
        };

        return new HashExecutor(hashAlgorithm).ExecuteAsync(content);
    }
}