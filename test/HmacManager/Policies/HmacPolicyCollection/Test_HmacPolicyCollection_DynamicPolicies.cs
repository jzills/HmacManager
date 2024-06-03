using HmacManager.Caching;
using HmacManager.Common;
using HmacManager.Common.Extensions;
using HmacManager.Components;
using HmacManager.Headers;
using HmacManager.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Tests.Policies;

[TestFixture]
public class Test_HmacPolicyCollection_DynamicPolicies : TestServiceCollection
{
    [Test]
    public void Test()
    {
        var policies = (HmacPolicyCollection)ServiceProvider.GetRequiredService<IComponentCollection<HmacPolicy>>();
        
        var headerSchemes = new HeaderSchemeCollection();
        var headerScheme = new HeaderScheme("Cool_Dynamic_Scheme");
        headerScheme.AddHeader("Cool_Dynamic_Header", "Cool_Dynamic_ClaimType");
        headerSchemes.Add(headerScheme);

        policies.Add(new HmacPolicy
        {
            Name = "Cool_Dynamic_Policy",
            Algorithms = new Algorithms
            {
                ContentHashAlgorithm = ContentHashAlgorithm.SHA1,
                SigningHashAlgorithm = SigningHashAlgorithm.HMACSHA1
            },
            Keys = new KeyCredentials
            {
                PublicKey = PublicKey,
                PrivateKey = PrivateKey
            },
            Nonce = new Nonce
            {
                CacheType = NonceCacheType.Memory,
                MaxAgeInSeconds = 30
            },
            HeaderSchemes = headerSchemes
        });

        policies = (HmacPolicyCollection)ServiceProvider.GetRequiredService<IComponentCollection<HmacPolicy>>();
        
        Assert.IsTrue(policies.TryGetValue("Cool_Dynamic_Policy", out var policy));
        Assert.IsTrue(policy.Name == "Cool_Dynamic_Policy");

    }
}