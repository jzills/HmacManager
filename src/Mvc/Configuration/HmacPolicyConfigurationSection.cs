using HmacManager.Policies;

namespace HmacManager.Mvc;

internal class HmacPolicyConfigurationSection : HmacPolicy
{
    public new IReadOnlyList<HeaderSchemeConfigurationSection>? HeaderSchemes { get; set; }
}