using HmacManagement.Remodel;

namespace HmacManagement.Components;

public class HmacProviderOptions
{
    public required KeyCredentials Keys { get; set; }
    public required Algorithms Algorithms { get; set; }
}