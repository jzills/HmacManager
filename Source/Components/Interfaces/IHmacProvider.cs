using HmacManagement.Headers;

namespace HmacManagement.Components;

/// <summary>
/// A class representing a HmacProvider, which
/// handles the computation of hashes and signatures.
/// </summary>
public interface IHmacProvider
{
    string ComputeContentHash(string content);
    string ComputeSignature(string signingContent);
    Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset dateRequested, 
        Guid nonce,
        HeaderValue[]? headerValues = null
    );
}
