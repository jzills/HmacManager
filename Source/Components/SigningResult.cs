namespace Source.Components;

public class SigningResult
{
    public required HMAC? HMAC { get; init; }
    public required bool IsSigned { get; init; }
}