using HmacManager.Components;
using HmacManager.Mvc.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Tests;

public class Test_ServiceCollection_AddHmacManager_SigningContentBuilder : TestBase
{
    [Test]
    [TestCaseSource(typeof(TestCaseSource), nameof(TestCaseSource.GetHttpRequestMessages))]
    public async Task Test(HttpRequestMessage request)
    {
        var services = new ServiceCollection()
            .AddMemoryCache() 
            .AddHmacManager(options =>
            {
                options.AddPolicy("MyPolicy_1", policy =>
                {
                    policy.UsePublicKey(PublicKey);
                    policy.UsePrivateKey(PrivateKey);
                    policy.UseMemoryCache(30);
                    policy.UseSigningContentBuilder(context => 
                    {
                        return "Test:1:2:3";
                    });
                });

                options.AddPolicy("MyPolicy_2", policy =>
                {
                    policy.UsePublicKey(PublicKey2);
                    policy.UsePrivateKey(PrivateKey2);
                    policy.UseMemoryCache(100);
                });
            });

        var serviceProvider = services.BuildServiceProvider();
        var hmacManagerFactory = serviceProvider.GetRequiredService<IHmacManagerFactory>();

        // Read request, parse auth header
        var hmacManager = hmacManagerFactory.Create("MyPolicy_1");
        var signingResult = await hmacManager.SignAsync(request);
        var verificationResult = await hmacManager.VerifyAsync(request);

        Assert.That(signingResult.Hmac, Is.Not.Null);
        Assert.That(signingResult.Hmac.SigningContent, Is.EqualTo("Test:1:2:3"));
        Assert.That(verificationResult.Hmac, Is.Not.Null);
        Assert.That(verificationResult.Hmac.SigningContent, Is.EqualTo("Test:1:2:3"));
        Assert.That(verificationResult.Hmac.Signature, Is.EqualTo(signingResult.Hmac.Signature));
    }
}