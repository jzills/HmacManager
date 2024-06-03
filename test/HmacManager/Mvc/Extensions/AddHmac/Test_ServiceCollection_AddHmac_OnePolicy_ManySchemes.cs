using Microsoft.Extensions.DependencyInjection;
using HmacManager.Mvc.Extensions;
using HmacManager.Components;

namespace Unit.Tests;

public class Test_ServiceCollection_AddHmac_OnePolicy_ManySchemes : TestBase
{
    [Test]
    [TestCaseSource(typeof(TestCaseSource), nameof(TestCaseSource.GetHttpRequestMessages))]
    public async Task Test(HttpRequestMessage request)
    {
        var services = new ServiceCollection()
            .AddStackExchangeRedisCache(options => options.Configuration = "127.0.0.1:6379");

        services
            .AddAuthentication()
            .AddHmac(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                {
                    policy.UsePublicKey(PublicKey);
                    policy.UsePrivateKey(PrivateKey);
                    policy.UseContentHashAlgorithm(ContentHashAlgorithm.SHA256);
                    policy.UseSigningHashAlgorithm(SigningHashAlgorithm.HMACSHA256);
                    policy.UseDistributedCache(30);
                
                    policy.AddScheme("MyScheme_1", scheme =>
                    {
                        scheme.AddHeader("MyHeader1_1");
                        scheme.AddHeader("MyHeader2_1");
                        scheme.AddHeader("MyHeader3_1");
                    });

                    policy.AddScheme("MyScheme_2", scheme =>
                    {
                        scheme.AddHeader("MyHeader1_2");
                        scheme.AddHeader("MyHeader2_2");
                        scheme.AddHeader("MyHeader3_2");
                    });
                });
            });

        var serviceProvider = services.BuildServiceProvider();
        var hmacManagerFactory = serviceProvider.GetRequiredService<IHmacManagerFactory>();
        Assert.IsNotNull(hmacManagerFactory, "An implementation of \"IHmacManagerFactory\" is required but has not been registered.");

        var hmacManager = hmacManagerFactory.Create("MyPolicy");
        Assert.IsNotNull(hmacManager, "An implementation of \"IHmacManager\" is required but has not been registered.");

        var signingResult = await hmacManager.SignAsync(request);
        Assert.IsTrue(signingResult.IsSuccess);
        Assert.IsTrue(signingResult.Policy == "MyPolicy");
        Assert.IsTrue(signingResult.Hmac?.HeaderValues?.Count() == 0);
        Assert.IsNull(signingResult.HeaderScheme);

        var verificationResult = await hmacManager.VerifyAsync(request);
        Assert.IsTrue(verificationResult.IsSuccess);
        Assert.IsTrue(verificationResult.Policy == "MyPolicy");
        Assert.IsTrue(verificationResult.Hmac?.HeaderValues?.Count() == 0);
        Assert.IsNull(verificationResult.HeaderScheme);
    }
}