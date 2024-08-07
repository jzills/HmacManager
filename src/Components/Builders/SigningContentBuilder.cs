using System.Text;
using HmacManager.Headers;

namespace HmacManager.Components;

public class SigningContentBuilder : ISigningContentBuilder
{
    protected readonly StringBuilder Builder;
    protected readonly SigningContentContext Context;
    
    internal SigningContentBuilder()
    {
        Context = new();
        Builder = new StringBuilder();
    }

    internal SigningContentBuilder WithRequest(HttpRequestMessage request)
    {
        Context.Request = request;
        return this;
    }  

    internal SigningContentBuilder WithPublicKey(Guid publicKey)
    {
        Context.PublicKey = publicKey;
        return this;
    }   

    internal SigningContentBuilder WithDateRequested(DateTimeOffset dateRequested)
    {
        Context.DateRequested = dateRequested;
        return this;
    }   

    internal SigningContentBuilder WithNonce(Guid nonce)
    {
        Context.Nonce = nonce;
        return this;
    }

    internal SigningContentBuilder WithHeaderValues(HeaderValue[] headerValues)
    {
        Context.HeaderValues = headerValues;
        return this;
    }

    internal SigningContentBuilder WithContentHash(string? contentHash)
    {
        Context.ContentHash = contentHash;
        return this;
    }

    public virtual SigningContentBuilder CreateBuilder() => new SigningContentBuilder();

    public virtual string Build()
    {
        ArgumentNullException.ThrowIfNull(Context.Request, nameof(Context.Request));

        Builder.Append($"{Context.Request.Method}");

        if (Context.Request.RequestUri is not null)
        {
            if (Context.Request.RequestUri.IsAbsoluteUri)
            {
                Builder.Append($":{Context.Request.RequestUri.PathAndQuery}");
                Builder.Append($":{Context.Request.RequestUri.Authority}");
            }
            else
            {
                // Handle the case when a relative uri is used, for instance,
                // when using an HttpClient with a predefined BaseAddress. For
                // cases like this, only append the path and any potential query
                // but disregard the authority.
                Builder.Append($":{Context.Request.RequestUri.OriginalString}");
            }
        }

        Builder.Append($":{Context.DateRequested?.ToUnixTimeMilliseconds()}");
        Builder.Append($":{Context.PublicKey}");

        if (Context.ContentHash is not null)
        {
            Builder.Append($":{Context.ContentHash}");
        }

        if (Context.HeaderValues?.Any() ?? false)
        {
            Builder.Append(":");
            Builder.AppendJoin(":", Context.HeaderValues.Select(element => element.Value));
        }

        Builder.Append($":{Context.Nonce}");
        return Builder.ToString();
    }
}