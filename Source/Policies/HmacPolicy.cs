using HmacManagement.Caching;
using HmacManagement.Headers;

namespace HmacManagement.Policies;

public class HmacPolicy
{
    public readonly string Name;
    public HmacPolicy(string name) => Name = name;
    public KeyCredentials Keys { get; set; } = new();
    public Algorithms Algorithms { get; set; } = new();
    public Nonce Nonce { get; set; } = new();
    public readonly HeaderSchemeCollection HeaderSchemes = new();
}