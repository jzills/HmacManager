using HmacManagement.Remodel;

namespace HmacManagement.Components;

public class HmacProviderOptions
{
    public KeyCredentials Keys { get; set; }
    public Algorithms Algorithms { get; set; }
}