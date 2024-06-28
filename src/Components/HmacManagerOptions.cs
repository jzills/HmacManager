using HmacManager.Headers;

namespace HmacManager.Components;

public class HmacManagerOptions
{
    public readonly string Policy;
    public HmacManagerOptions(string policy) => Policy = policy;
    public HeaderScheme? HeaderScheme { get; set; }
    public int MaxAgeInSeconds { get; set; } = 30;
}
