using System.Text;
using HmacManager.Extensions;
using HmacManager.Headers;
using HmacManager.Mvc.Extensions.Internal;

namespace HmacManager.Components;

public class HmacProvider : IHmacProvider
{
    protected readonly HmacProviderOptions Options;

    public HmacProvider(HmacProviderOptions options) => Options = options;

    public Task<string> ComputeSignatureAsync(string signingContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(signingContent);

        return Options.SignatureHashGenerator.HashAsync(signingContent);
    }

    public async Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset dateRequested, 
        Guid nonce,
        HeaderValue[]? headerValues = null
    )
    {
        var builder = Options.SigningContentBuilder.CreateBuilder()
            .WithRequest(request)
            .WithPublicKey(Options.Keys.PublicKey)
            .WithDateRequested(dateRequested)
            .WithNonce(nonce)
            .WithHeaderValues(headerValues ?? []);

        if (request.TryCopyAndAssignContent(out var stream) && 
            stream.TryReadAndResetPosition(out var contentString))
        {              
            var contentHash = await Options.ContentHashGenerator.HashAsync(contentString);
            builder.WithContentHash(contentHash);
        }

        return builder.Build();
    }
}
