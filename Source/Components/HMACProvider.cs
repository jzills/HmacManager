using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Source.Components;

public class HMACProviderOptions
{
    public required string ClientId { get; set; } 
    public required string ClientSecret { get; set; }
}

public interface IHMACProvider
{
    string ComputeContentHash(string content);
    string ComputeSignature(string signingContent);
    Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset requestedOn, 
        Guid nonce,
        MessageContent[]? additionalContent = null
    );
}

public class HMACProvider : IHMACProvider
{
    private readonly HMACProviderOptions _options;

    public HMACProvider(HMACProviderOptions options) => _options = options;

    public string ComputeContentHash(string content)
    {
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var hashBytes = SHA256.Create().ComputeHash(contentBytes);
        return Convert.ToBase64String(hashBytes);
    }

    public string ComputeSignature(string signingContent)
    {
        var hashBytes = new HMACSHA256(Convert.FromBase64String(_options.ClientSecret))
            .ComputeHash(Encoding.UTF8.GetBytes(signingContent));

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