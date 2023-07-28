using System.Security.Cryptography;
using System.Text;

namespace HmacManagement.Components;

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
            SignatureHashAlgorithm.HMACSHA1   => new HMACSHA1  (secretBytes).ComputeHash(signingContentBytes),
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
        Header[]? signedHeaders = null
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

        macBuilder.Append($":{requestedOn}");
        macBuilder.Append($":{_options.ClientId}");

        if (request.Content is not null && request.Content.Headers.ContentLength > 0)
        {
            // REMEMBER:
            // As it stands, the HttpRequest from AspNetCore must
            // have buffering enabled and rewind the body stream after
            // verifying the signature.

            // Ideally, a way to handle this internally would be preferred.
            // One potential solution would be to have overloads to handle
            // HttpRequest objects.

            // SOLUTION 1
            // REQUIRES TESTING
            using var stream = new MemoryStream();
            await request.Content.CopyToAsync(stream);
            
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                throw new Exception();
            }

            var content = await new StreamReader(stream).ReadToEndAsync();
            var contentHash = ComputeContentHash(content);
            macBuilder.Append($":{contentHash}");

            // var contentHash = ComputeContentHash(await request.Content.ReadAsStringAsync());
            // macBuilder.Append($":{contentHash}");
        }

        if (signedHeaders is not null)
        {
            macBuilder.AppendJoin(":", signedHeaders.Select(element => element.Value));
        }

        macBuilder.Append($":{nonce}");
        return macBuilder.ToString();
    }
}