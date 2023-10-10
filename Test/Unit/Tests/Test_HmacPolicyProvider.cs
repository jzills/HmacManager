using System.Text;
using HmacManagement.Caching;
using HmacManagement.Components;
using HmacManagement.Remodel;

namespace Unit.Tests;

public class Test_HmacPolicyProvider
{
    [Test]
    [TestCaseSource(typeof(DataSource_HmacPolicies), nameof(DataSource_HmacPolicies.GetPolicies))]
    public void Init_Provider_Returns_SamePolicies(IDictionary<string, HmacPolicy> policies)
    {
        var provider = new HmacPolicyProvider(policies);
        foreach (var policyName in policies.Keys)
        {
            var policy = provider.Get(policyName);
            if (policy is not null)
            {
                var sourcePolicy = policies[policyName];

                Assert.True(policy!.Algorithms.ContentAlgorithm == sourcePolicy.Algorithms.ContentAlgorithm);
                Assert.True(policy!.Algorithms.SigningAlgorithm == sourcePolicy.Algorithms.SigningAlgorithm);
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