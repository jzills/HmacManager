namespace HmacManager.Components;

public class HmacManagerOptions
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public string[] SignedHeaders { get; set; } = Array.Empty<string>();
}