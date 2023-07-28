using HmacManagement.Caching;
using HmacManagement.Components;
using HmacManagement.Headers;

namespace HmacManagement.Mvc;

public class HmacOptions
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public string[] SignedHeaders { get; set; } = Array.Empty<string>();
    public ContentHashAlgorithm ContentHashAlgorithm { get; set; } = ContentHashAlgorithm.SHA256;
    public SignatureHashAlgorithm SignatureHashAlgorithm { get; set; } = SignatureHashAlgorithm.HMACSHA256;
    public NonceCacheType NonceCacheType { get; set; } = NonceCacheType.Memory;
    public HeaderSchemeCollection HeaderSchemes { get; } = new();
}