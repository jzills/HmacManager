using HmacManager.Caching;
using HmacManager.Headers;

namespace HmacManager.Policies;

public class HmacPolicy
{
    internal readonly string Name;
    public HmacPolicy() => Name = string.Empty;
    public HmacPolicy(string name) => Name = name;
    public KeyCredentials Keys { get; set; } = new();
    public Algorithms Algorithms { get; set; } = new();
    public Nonce Nonce { get; set; } = new();
    public HeaderSchemeCollection HeaderSchemes { get; set; } = new();
}