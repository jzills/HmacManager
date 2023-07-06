namespace Source.Components;

public class VerificationResult
{
    public HMAC? HMAC { get; init; } = default;
    public bool IsTrusted { get; init; } = false;
}