using HmacManagement.Components;
using HmacManagement.Policies;
using Unit.Mocks;

namespace Unit.Tests;

public class Test_HmacManagerFactory
{
    [Test]
    [TestCaseSource(typeof(DataSource_HmacPolicies), nameof(DataSource_HmacPolicies.GetPolicies))]
    public void Init_HmacManagerFactory_Create_HmacManager(IDictionary<string, HmacPolicy> policies)
    {
        var factory = new HmacManagerFactory(
            new HmacPolicyProvider(policies),
            new NonceCacheProviderMock()
        );

        Assert.DoesNotThrow(() => factory.Create());

        foreach (var policy in policies)
        {
            Assert.DoesNotThrow(() => factory.Create(policy.Key));

            var schemes = policy.Value.GetHeaderSchemes();
            if (schemes.Any())
            {
                foreach (var scheme in schemes)
                {
                    Assert.DoesNotThrow(() => factory.Create(policy.Key, scheme.Key));
                }
            }
        }
    } 
}