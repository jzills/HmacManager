using HmacManagement.Headers;

namespace HmacManagement.Components;

public class Hmac
{
    public string Signature { get; set; } = string.Empty;
    public DateTimeOffset RequestedOn { get; init; } = DateTimeOffset.UtcNow;
    public Guid Nonce { get; init; } = Guid.NewGuid();
    public string? SigningContent { get; set; }
    public HeaderValue[] HeaderValues { get; set; } = new HeaderValue[0];
}
