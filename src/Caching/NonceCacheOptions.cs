namespace HmacManager.Caching;

internal class NonceCacheOptions
{
    public int MaxAgeInSeconds { get; set; } = 30;
    public NonceCacheType CacheType { get; set; } = NonceCacheType.Memory;
    public string GetKey(Guid nonce) => $"{typeof(Components.HmacManager).Name}:{Enum.GetName(CacheType)}:{nonce}";
}