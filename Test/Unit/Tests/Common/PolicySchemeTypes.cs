namespace Unit.Tests;

public sealed class PolicySchemeType
{
    public readonly string Policy;
    public readonly string Scheme;

    public static readonly PolicySchemeType Policy_InMemory             = new PolicySchemeType(nameof(Policy_InMemory));
    public static readonly PolicySchemeType Policy_InMemory_Scheme_1    = new PolicySchemeType(nameof(Policy_InMemory), nameof(Policy_InMemory_Scheme_1));
    public static readonly PolicySchemeType Policy_InMemory_Scheme_2    = new PolicySchemeType(nameof(Policy_InMemory), nameof(Policy_InMemory_Scheme_2));
    public static readonly PolicySchemeType Policy_InMemory_Scheme_3    = new PolicySchemeType(nameof(Policy_InMemory), nameof(Policy_InMemory_Scheme_3));
    public static readonly PolicySchemeType Policy_Distributed          = new PolicySchemeType(nameof(Policy_Distributed));
    public static readonly PolicySchemeType Policy_Distributed_Scheme_1 = new PolicySchemeType(nameof(Policy_Distributed), nameof(Policy_Distributed_Scheme_1));
    public static readonly PolicySchemeType Policy_Distributed_Scheme_2 = new PolicySchemeType(nameof(Policy_Distributed), nameof(Policy_Distributed_Scheme_2));
    public static readonly PolicySchemeType Policy_Distributed_Scheme_3 = new PolicySchemeType(nameof(Policy_Distributed), nameof(Policy_Distributed_Scheme_3));

    private PolicySchemeType(string policy, string scheme = null!) => 
        (Policy, Scheme) = (policy, scheme);
}