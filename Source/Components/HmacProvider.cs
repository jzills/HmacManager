using System.Security.Cryptography;
using System.Text;
using HmacManagement.Headers;

namespace HmacManagement.Components;

public class HmacProvider : IHmacProvider
{
    private readonly HmacProviderOptions _options;

    public HmacProvider(HmacProviderOptions options) => _options = options;

    public string ComputeContentHash(string content)
    {
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var hashBytes = _options.Algorithms.ContentHashAlgorithm switch
        {
            ContentHashAlgorithm.SHA1   => SHA1  .Create().ComputeHash(contentBytes),
            ContentHashAlgorithm.SHA256 => SHA256.Create().ComputeHash(contentBytes),
            ContentHashAlgorithm.SHA512 => SHA512.Create().ComputeHash(contentBytes),
            _                           => SHA256.Create().ComputeHash(contentBytes)
        };

        return Convert.ToBase64String(hashBytes);
    }

    public string ComputeSignature(string signingContent)
    {
        var secretBytes = Convert.FromBase64String(_options.Keys.PrivateKey);
        var signingContentBytes = Encoding.UTF8.GetBytes(signingContent);
        var hashBytes = _options.Algorithms.SigningHashAlgorithm switch
        {
            SigningHashAlgorithm.HMACSHA1   => new HMACSHA1  (secretBytes).ComputeHash(signingContentBytes),
            SigningHashAlgorithm.HMACSHA256 => new HMACSHA256(secretBytes).ComputeHash(signingContentBytes),
            SigningHashAlgorithm.HMACSHA512 => new HMACSHA512(secretBytes).ComputeHash(signingContentBytes),
            _                               => new HMACSHA256(secretBytes).ComputeHash(signingContentBytes)
        };
        
        return Convert.ToBase64String(hashBytes);
    }

    public async Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset dateRequested, 
        Guid nonce,
        HeaderValue[]? headerValues = null
    )
    {
        var macBuilder = new StringBuilder($"{request.Method}");

        if (request.RequestUri is not null)
        {
            if (request.RequestUri.IsAbsoluteUri)
            {
                macBuilder.Append($":{request.RequestUri.PathAndQuery}");
                macBuilder.Append($":{request.RequestUri.Authority}");
            }
            else
            {
                // Handle the case when a relative uri is used, for instance,
                // when using an HttpClient with a predefined BaseAddress. For
                // cases like this, only append the path and any potential query
                // but disregard the authority.
                macBuilder.Append($":{request.RequestUri.OriginalString}");
            }
        }

        macBuilder.Append($":{dateRequested}");
        macBuilder.Append($":{_options.Keys.PublicKey}");

        if (request.Content is not null && request.Content.Headers.ContentLength > 0)
        {
            var contentHash = ComputeContentHash(await request.Content.ReadAsStringAsync());
            macBuilder.Append($":{contentHash}");
        }

        if (headerValues?.Any() ?? false)
        {
            macBuilder.Append(":");
            macBuilder.AppendJoin(":", headerValues.Select(element => element.Value));
        }

        macBuilder.Append($":{nonce}");
        return macBuilder.ToString();
    }
}
