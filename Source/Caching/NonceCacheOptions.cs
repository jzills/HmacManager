namespace HmacManagement.Caching;

public class NonceCacheOptions
{
    public TimeSpan MaxAge { get; init; }
    public NonceCacheType Type { get; init; }
}