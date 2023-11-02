using HmacManagement.Headers;

namespace HmacManagement.Components;

public interface IHmacProvider
{
    string ComputeContentHash(string content);
    string ComputeSignature(string signingContent);
    Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset DateRequested, 
        Guid nonce,
        HeaderValue[]? headerValues = null
    );
}
