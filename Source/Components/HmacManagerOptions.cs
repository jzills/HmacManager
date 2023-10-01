using HmacManagement.Policies;

namespace HmacManagement.Components;

public class HmacManagerOptions
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
}
