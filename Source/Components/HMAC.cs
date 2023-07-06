namespace Source.Components;

public class HMAC
{
    public string? Signature { get; set; }
    public DateTimeOffset RequestedOn { get; init; } = DateTimeOffset.UtcNow;
    public Guid Nonce { get; init; } = Guid.NewGuid();
    public string? SigningContent { get; set; }
    public MessageContent[]? MessageContent { get; set; }
}