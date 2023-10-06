using HmacManagement.Remodel;

namespace HmacManagement.Components;

public interface IHmacProvider
{
    string ComputeContentHash(string content);
    string ComputeSignature(string signingContent);
    Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset requestedOn, 
        Guid nonce,
        HeaderValue[]? headerValues = null
    );
}
