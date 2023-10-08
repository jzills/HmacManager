using HmacManagement.Policies;
using HmacManagement.Remodel;

namespace HmacManagement.Components;

public class HmacManagerOptions
{
    public TimeSpan MaxAge { get; set; } = TimeSpan.FromSeconds(30);
    public HeaderScheme? HeaderScheme { get; set; }
}
