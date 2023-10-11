namespace HmacManagement.Caching;

public class Nonce
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public NonceCacheType CacheType { get; set; } = NonceCacheType.Memory;
}