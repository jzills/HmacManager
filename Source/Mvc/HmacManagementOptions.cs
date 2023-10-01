using HmacManagement.Caching;
using HmacManagement.Components;

namespace HmacManagement.Mvc;

public class HmacManagementOptions
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public ContentHashAlgorithm ContentHashAlgorithm { get; set; } = ContentHashAlgorithm.SHA256;
    public SignatureHashAlgorithm SignatureHashAlgorithm { get; set; } = SignatureHashAlgorithm.HMACSHA256;
    public NonceCacheType NonceCacheType { get; set; } = NonceCacheType.Memory;
}