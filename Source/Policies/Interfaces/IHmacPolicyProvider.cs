namespace HmacManagement.Policies;

public interface IHmacPolicyProvider
{
    HmacPolicy? GetPolicy(string name);
    IReadOnlyCollection<HmacPolicy> GetPolicies();
}