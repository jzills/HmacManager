using HmacManager.Headers;

namespace HmacManager.Components;

public class SigningContentContext
{
    public HttpRequestMessage? Request { get; set; }
    public Guid? PublicKey { get; set; }
    public DateTimeOffset? DateRequested { get; set; }
    public Guid? Nonce { get; set; }
    public HeaderValue[] HeaderValues { get; set; } = [];
    public string? ContentHash { get; set; }
}