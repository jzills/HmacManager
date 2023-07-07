namespace Source.Components;

public class HMACManagerOptions
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public string[] MessageContentHeaders { get; set; } = Array.Empty<string>();
}