namespace HmacManagement.Caching;

public class Nonce
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public string CacheName { get; set; } = string.Empty;
}