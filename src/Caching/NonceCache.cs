namespace HmacManager.Caching;

internal abstract class NonceCache : INonceCache
{
    protected readonly NonceCacheOptions Options;
    protected NonceCache(NonceCacheOptions options) => Options = options;
    
    public abstract Task<bool> ContainsAsync(Guid nonce);
    public abstract Task SetAsync(Guid nonce, DateTimeOffset dateRequested);

    protected string Key(Guid nonce) => Options.CreateKey(nonce);
    protected DateTimeOffset AbsoluteExpiration(DateTimeOffset dateRequested) => dateRequested.AddSeconds(Options.MaxAgeInSeconds);
}