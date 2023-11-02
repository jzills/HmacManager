using HmacManagement.Headers;

namespace HmacManagement.Components;

public class HmacManagerOptions
{
    public readonly string Policy;
    public HmacManagerOptions(string policy) => Policy = policy;
    public HeaderScheme? HeaderScheme { get; set; }
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
}
