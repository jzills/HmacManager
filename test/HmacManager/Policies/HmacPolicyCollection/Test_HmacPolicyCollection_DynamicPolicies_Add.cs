using Microsoft.Extensions.DependencyInjection;
using HmacManager.Caching;
using HmacManager.Common.Extensions;
using HmacManager.Components;
using HmacManager.Headers;
using HmacManager.Policies;
using HmacManager.Policies.Extensions;

namespace Unit.Tests.Policies;

[TestFixture]
public class Test_HmacPolicyCollection_DynamicPolicies_Add : TestServiceCollection
{
    [Test]
    public void Test_AddPolicy_VerifyThrough_HmacPolicyCollection()
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

    [Test]
    public async Task Test_AddPolicy_VerifyThrough_HmacManagerFactory()
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

        var hmacManagerFactory = ServiceProvider.GetRequiredService<IHmacManagerFactory>();
        var hmacManager = hmacManagerFactory.Create("Cool_Dynamic_Policy", "Cool_Dynamic_Scheme");

        Assert.IsNotNull(hmacManager);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/some/path");
        request.Headers.Add("Cool_Dynamic_Header", "Cool_Dynamic_Value");

        var signingResult = await hmacManager.SignAsync(request);

        Assert.IsTrue(signingResult.IsSuccess);
        Assert.IsTrue(signingResult.Policy == "Cool_Dynamic_Policy");
        Assert.IsTrue(signingResult.HeaderScheme == "Cool_Dynamic_Scheme");
    }
}