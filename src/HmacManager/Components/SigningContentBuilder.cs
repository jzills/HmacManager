using System.Text;
using HmacManager.Headers;

namespace HmacManager.Components;

internal class SigningContentBuilder
{
    protected readonly HttpRequestMessage Request;
    protected readonly StringBuilder Builder;
    protected Guid PublicKey;
    protected DateTimeOffset DateRequested;
    protected Guid Nonce;
    protected HeaderValue[] HeaderValues;
    protected string? ContentHash;

    public SigningContentBuilder(HttpRequestMessage request)
    {
        Request = request;
        Builder = new StringBuilder($"{Request.Method}");
        HeaderValues = [];
    }

    public SigningContentBuilder WithPublicKey(Guid publicKey)
    {
        PublicKey = publicKey;
        return this;
    }   

    public SigningContentBuilder WithDateRequested(DateTimeOffset dateRequested)
    {
        DateRequested = dateRequested;
        return this;
    }   

    public SigningContentBuilder WithNonce(Guid nonce)
    {
        Nonce = nonce;
        return this;
    }

    public SigningContentBuilder WithHeaderValues(HeaderValue[] headerValues)
    {
        HeaderValues = headerValues;
        return this;
    }

    public SigningContentBuilder WithContentHash(string? contentHash)
    {
        ContentHash = contentHash;
        return this;
    }

    public virtual string Build()
    {
        if (Request.RequestUri is not null)
        {
            if (Request.RequestUri.IsAbsoluteUri)
            {
                Builder.Append($":{Request.RequestUri.PathAndQuery}");
                Builder.Append($":{Request.RequestUri.Authority}");
            }
            else
            {
                // Handle the case when a relative uri is used, for instance,
                // when using an HttpClient with a predefined BaseAddress. For
                // cases like this, only append the path and any potential query
                // but disregard the authority.
                Builder.Append($":{Request.RequestUri.OriginalString}");
            }
        }

        Builder.Append($":{DateRequested}");
        Builder.Append($":{PublicKey}");

        if (ContentHash is not null)
        {
            Builder.Append($":{ContentHash}");
        }

        if (HeaderValues?.Any() ?? false)
        {
            Builder.Append(":");
            Builder.AppendJoin(":", HeaderValues.Select(element => element.Value));
        }

        Builder.Append($":{Nonce}");
        return Builder.ToString();
    }
}