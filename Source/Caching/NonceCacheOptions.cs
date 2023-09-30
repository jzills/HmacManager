namespace HmacManagement.Caching;

public class NonceCacheOptions
{
    public TimeSpan MaxAge { get; set; }
    public NonceCacheType Type { get; set; }
}