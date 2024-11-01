using HmacManager.Extensions;
using HmacManager.Headers;
using HmacManager.Mvc.Extensions.Internal;

namespace HmacManager.Components;

public class HmacSignatureProvider : IHmacSignatureProvider
{
    protected readonly HmacSignatureProviderOptions Options;

    public HmacSignatureProvider(HmacSignatureProviderOptions options) => Options = options;

    public Task<string> ComputeSignatureAsync(string signingContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(signingContent);

        return Options.SignatureHashGenerator.HashAsync(signingContent);
    }

    public async Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset dateRequested, 
        Guid nonce,
        IReadOnlyCollection<HeaderValue>? headerValues = null
    )
    {
        var builder = Options.SigningContentBuilder.CreateBuilder()
            .WithRequest(request)
            .WithPublicKey(Options.Keys.PublicKey)
            .WithDateRequested(dateRequested)
            .WithNonce(nonce)
            .WithHeaderValues(headerValues ?? []);

        if (request.HasContent())
        {
            var contentString = await request.Content!.ReadAsStringAsync();            
            var contentHash = await Options.ContentHashGenerator.HashAsync(contentString);
            builder.WithContentHash(contentHash);
        }

        return builder.Build();
    }
}
