using System.Security.Cryptography;
using System.Text;

namespace HmacManager.Components;

public class HmacProvider : IHmacProvider
{
    private readonly HmacProviderOptions _options;

    public HmacProvider(HmacProviderOptions options) => _options = options;

    public string ComputeContentHash(string content)
    {
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var hashBytes = _options.ContentHashAlgorithm switch
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
        var secretBytes = Convert.FromBase64String(_options.ClientSecret);
        var signingContentBytes = Encoding.UTF8.GetBytes(signingContent);
        var hashBytes = _options.SignatureHashAlgorithm switch
        {
            SignatureHashAlgorithm.HMACSHA1   => new HMACSHA1(secretBytes)  .ComputeHash(signingContentBytes),
            SignatureHashAlgorithm.HMACSHA256 => new HMACSHA256(secretBytes).ComputeHash(signingContentBytes),
            SignatureHashAlgorithm.HMACSHA512 => new HMACSHA512(secretBytes).ComputeHash(signingContentBytes),
            _                                 => new HMACSHA256(secretBytes).ComputeHash(signingContentBytes)
        };
        
        return Convert.ToBase64String(hashBytes);
    }

    public async Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset requestedOn, 
        Guid nonce,
        MessageContent[]? messageContentHeaders = null
    )
    {
        var macBuilder = new StringBuilder($"{request.Method}");

        if (request.RequestUri is not null)
        {
            macBuilder.Append($":{request.RequestUri.PathAndQuery}");
            macBuilder.Append($":{request.RequestUri.Authority}");
        }

        macBuilder.Append($":{requestedOn}");
        macBuilder.Append($":{_options.ClientId}");

        if (request.Content is not null && request.Content.Headers.ContentLength > 0)
        {
            var contentHash = ComputeContentHash(await request.Content.ReadAsStringAsync());
            macBuilder.Append($":{contentHash}");
        }

        if (messageContentHeaders is not null)
        {
            macBuilder.AppendJoin(":", messageContentHeaders.Select(element => element.Value));
        }

        macBuilder.Append($":{nonce}");
        return macBuilder.ToString();
    }
}