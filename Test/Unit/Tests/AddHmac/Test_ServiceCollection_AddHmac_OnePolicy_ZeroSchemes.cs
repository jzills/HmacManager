using Microsoft.Extensions.DependencyInjection;
using HmacManagement.Mvc.Extensions;
using HmacManagement.Components;

namespace Unit.Tests;

public class Test_ServiceCollection_AddHmac_OnePolicy_ZeroSchemes : TestBase
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
                    policy.UseDistributedCache(TimeSpan.FromMinutes(5));
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