using HmacManager.Extensions;
using HmacManager.Headers;

namespace HmacManager.Components;

public class HmacProvider : IHmacProvider
{
    private readonly ContentHashGenerator _contentHashGenerator;
    private readonly SignatureHashGenerator _signatureHashGenerator;

    public HmacProvider(
        ContentHashGenerator contentHashGenerator,
        SignatureHashGenerator signatureHashGenerator
    )
    {
        _contentHashGenerator = contentHashGenerator;
        _signatureHashGenerator = signatureHashGenerator;
    }

    public Task<string> ComputeSignatureAsync(string signingContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(signingContent);

        return _signatureHashGenerator.HashAsync(signingContent);
    }

    public async Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset dateRequested, 
        Guid nonce,
        HeaderValue[]? headerValues = null)
    {
        var builder = new SigningContentBuilder(request)
            .WithDateRequested(dateRequested)
            .WithNonce(nonce)
            .WithHeaderValues(headerValues);

        if (request.TryGetContent(out var content))
        {
            var contentString = await content.ReadAsStringAsync();
            var contentHash = await _contentHashGenerator.HashAsync(contentString);
            builder.WithContentHash(contentHash);
        }

        return builder.Build();
    }
}