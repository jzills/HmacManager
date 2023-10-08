using HmacManagement.Components;
using HmacManagement.Remodel;

namespace Unit;

public class UnitTest1
{
    public UnitTest1()
    {

    }

    [Test]
    [TestCaseSource(nameof(GetPolicies))]
    public async Task Get_Policies_From_Provider(IDictionary<string, HmacPolicy> policies)
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

    public static IDictionary<string, HmacPolicy>[] GetPolicies() =>
        new Dictionary<string, HmacPolicy>[]
        {
            new Dictionary<string, HmacPolicy>
            {
                {"AccountPolicy", new HmacPolicy 
                {
                    Algorithms = new Algorithms
                    {
                        ContentAlgorithm = ContentHashAlgorithm.SHA1,
                        SigningAlgorithm = SigningHashAlgorithm.HMACSHA1
                    },
                    Keys = new KeyCredentials
                    {
                        PrivateKey = "",
                        PublicKey = Guid.NewGuid()
                    }
                }},
                {"UserPolicy", new HmacPolicy {}},
                {"EmailPolicy", new HmacPolicy {}}
            }
        };
}