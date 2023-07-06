namespace Source.Components;

public interface IHMACManager
{
    Task<VerificationResult> VerifyAsync(HttpRequestMessage request);
    Task<SigningResult> SignAsync(
        HttpRequestMessage request, 
        MessageContent[]? additionalContent = null
    );
}