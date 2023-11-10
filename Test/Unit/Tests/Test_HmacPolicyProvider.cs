using HmacManagement.Policies;

namespace Unit.Tests;

public class Test_HmacPolicyProvider
{
    [Test]
    [TestCaseSource(typeof(DataSource_HmacPolicies), nameof(DataSource_HmacPolicies.GetPolicies))]
    public void Init_Provider_Returns_SamePolicies(IDictionary<string, HmacPolicy> policies)
    {
        var provider = new HmacPolicyCollection();
        foreach (var policyName in policies.Keys)
        {
            var policy = provider.GetPolicy(policyName);
            if (policy is not null)
            {
                var sourcePolicy = policies[policyName];

                Assert.True(policy!.Algorithms.ContentHashAlgorithm == sourcePolicy.Algorithms.ContentHashAlgorithm);
                Assert.True(policy!.Algorithms.SigningHashAlgorithm == sourcePolicy.Algorithms.SigningHashAlgorithm);
                Assert.True(policy!.Keys.PrivateKey == sourcePolicy.Keys.PrivateKey);
                Assert.True(policy!.Keys.PublicKey == sourcePolicy.Keys.PublicKey);
                Assert.True(policy!.Nonce.CacheType == sourcePolicy.Nonce.CacheType);
                Assert.True(policy!.Nonce.MaxAge == sourcePolicy.Nonce.MaxAge);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}