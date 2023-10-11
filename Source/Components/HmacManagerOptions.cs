using HmacManagement.Headers;

namespace HmacManagement.Components;

public class HmacManagerOptions
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public HeaderScheme? HeaderScheme { get; set; }
}
