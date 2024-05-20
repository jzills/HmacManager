namespace HmacManager.Caching;

internal class NonceCacheOptions
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromMinutes(5);
    public NonceCacheType CacheType { get; set; } = NonceCacheType.Memory;
    public string GetKey(Guid nonce) => $"{typeof(Components.HmacManager).Name}:{Enum.GetName(CacheType)}:{nonce}";
}