namespace HmacManager.Components;

public interface IHmacProvider
{
    string ComputeContentHash(string content);
    string ComputeSignature(string signingContent);
    Task<string> ComputeSigningContentAsync(
        HttpRequestMessage request, 
        DateTimeOffset requestedOn, 
        Guid nonce,
        Header[]? headersToSign = null
    );
}