namespace HmacManager.Caching;

internal class NonceCacheOptions
{
    public int MaxAgeInSeconds { get; set; } = 30;
    public NonceCacheType CacheType { get; set; } = NonceCacheType.Memory;
    public string CreateKey(Guid nonce) => $"{nameof(HmacManager)}:{Enum.GetName(CacheType)}:{nonce}";
}