using HmacManager.Headers;

namespace HmacManager.Components;

/// <summary>
/// A class representing a HmacProvider, which
/// handles the computation of hashes and signatures.
/// </summary>
public interface IHmacProvider
{
    Task<string> ComputeSignatureAsync(string signingContent);
    Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset dateRequested, 
        Guid nonce,
        HeaderValue[]? headerValues = null
    );
}
