namespace HmacManagement.Caching;

public class NonceCacheOptions
{
    public TimeSpan MaxAge { get; set; }
    public string CacheName { get; set; }
}