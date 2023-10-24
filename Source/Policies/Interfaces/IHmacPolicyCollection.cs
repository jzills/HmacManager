namespace HmacManagement.Policies;

public interface IHmacPolicyCollection
{
    void AddPolicy(string name, Action<HmacPolicy> configurePolicy);
    HmacPolicy? GetPolicy(string name);
    IReadOnlyCollection<HmacPolicy> GetPolicies();
}