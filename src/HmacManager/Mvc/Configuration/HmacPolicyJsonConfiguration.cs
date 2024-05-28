using HmacManager.Caching;
using HmacManager.Policies;

namespace HmacManager.Mvc;

internal class HmacPolicyJsonConfiguration
{
    public string? Name { get; set; }
    public KeyCredentials? Keys { get; set; }
    public Algorithms? Algorithms { get; set; }
    public Nonce? Nonce { get; set; }
    public List<HeaderSchemeJsonConfiguration>? HeaderSchemes { get; set; }
}