using System.Security.Cryptography;
using System.Text;

namespace Source.Components;

public class HMACProvider : IHMACProvider
{
    private readonly HMACProviderOptions _options;

    public HMACProvider(HMACProviderOptions options) => _options = options;

    public string ComputeContentHash(string content)
    {
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var hashBytes = _options.ContentHashAlgorithm switch
        {
            ContentHashAlgorithm.SHA1   => SHA1.Create().ComputeHash(contentBytes),
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
            SignatureHashAlgorithm.HMACSHA1   => new HMACSHA1(secretBytes).ComputeHash(signingContentBytes),
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
        MessageContent[]? additionalContent = null
    )
    {
        var macBuilder = new StringBuilder($"{request.Method}");

        if (request.RequestUri is Uri requestUri)
        {
            macBuilder.Append($":{requestUri.PathAndQuery}");
            macBuilder.Append($":{requestUri.Authority}");
        }

        macBuilder.Append($":{requestedOn}");
        macBuilder.Append($":{_options.ClientId}");

        if (request.Content is HttpContent content)
        {
            var contentHash = ComputeContentHash(await content.ReadAsStringAsync());
            macBuilder.Append($":{contentHash}");
        }

        if (additionalContent is MessageContent[] messageContent)
        {
            macBuilder.AppendJoin(":", messageContent.Select(element => element.Value));
        }

        macBuilder.Append($":{nonce}");
        return macBuilder.ToString();
    }
}