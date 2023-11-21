namespace HmacManagement.Caching;

public class NonceCacheOptions
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromMinutes(5);
    public string CacheName { get; set; } = "InMemory";
}