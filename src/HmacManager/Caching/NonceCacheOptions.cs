namespace HmacManager.Caching;

internal class NonceCacheOptions
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromMinutes(5);
    public string CacheName { get; set; } = Enum.GetName(NonceCacheType.Memory)!;
    public string GetKey(Guid nonce) => $"{typeof(Components.HmacManager).Name}:{CacheName}:{nonce}";
}