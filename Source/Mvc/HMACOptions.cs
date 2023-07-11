using HmacManager.Caching;
using HmacManager.Components;

namespace HmacManager.Mvc;

public class HmacOptions
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public string[] MessageContentHeaders { get; set; } = Array.Empty<string>();
    public ContentHashAlgorithm ContentHashAlgorithm { get; set; } = ContentHashAlgorithm.SHA256;
    public SignatureHashAlgorithm SignatureHashAlgorithm { get; set; } = SignatureHashAlgorithm.HMACSHA256;
    public NonceCacheType NonceCacheType { get; set; } = NonceCacheType.Distributed;
}