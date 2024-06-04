using HmacManager.Caching;
using HmacManager.Common.Extensions;
using HmacManager.Components;
using HmacManager.Headers;
using HmacManager.Policies;
using HmacManager.Policies.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Tests.Policies;

[TestFixture]
public class Test_HmacPolicyCollection_DynamicPolicies : TestServiceCollection
{
    [Test]
    public void Test()
    {
        var policies = ServiceProvider.GetRequiredService<IHmacPolicyCollection>();
        
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

        policies = ServiceProvider.GetRequiredService<IHmacPolicyCollection>();
        
        Assert.IsTrue(policies.TryGetValue("Cool_Dynamic_Policy", out var policy));
        Assert.IsTrue(policy.Name == "Cool_Dynamic_Policy");
    }
}