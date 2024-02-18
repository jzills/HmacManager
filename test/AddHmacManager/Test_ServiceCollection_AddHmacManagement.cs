using HmacManager.Components;
using HmacManager.Mvc.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Tests;

public class Test_ServiceCollection_AddHmacManager : TestBase
{
    [Test]
    [TestCaseSource(typeof(TestCaseSource), nameof(TestCaseSource.GetHttpRequestMessages))]
    public async Task Test(HttpRequestMessage request)
    {
        var services = new ServiceCollection()
            .AddMemoryCache() 
            // TODO: Handle error when AddMemoryCache isn't called
            // and HmacManager attempts to use IMemoryCache through UseMemoryCache call.
            .AddHmacManager(options =>
            {
                options.AddPolicy("MyPolicy_1", policy =>
                {
                    policy.UsePublicKey(PublicKey);
                    policy.UsePrivateKey(PrivateKey);
                    policy.UseMemoryCache(TimeSpan.FromSeconds(30));
                });

                options.AddPolicy("MyPolicy_2", policy =>
                {
                    policy.UsePublicKey(PublicKey2);
                    policy.UsePrivateKey(PrivateKey2);
                    policy.UseMemoryCache(TimeSpan.FromSeconds(30));
                });
            });

        var serviceProvider = services.BuildServiceProvider();
        var hmacManagerFactory = serviceProvider.GetRequiredService<IHmacManagerFactory>();

        // Read request, parse auth header
        var hmacManager1 = hmacManagerFactory.Create("MyPolicy_1");
        var signingResult1 = await hmacManager1.SignAsync(request);
        // var verifyResult1 = await hmacManager1.VerifyAsync(request);
        // Assert.True(signingResult1.Hmac.Signature == verifyResult1.Hmac.Signature);
        // Assert.True(signingResult1.IsSuccess && verifyResult1.IsSuccess);

        var hmacManager2 = hmacManagerFactory.Create("MyPolicy_2");
        // var signingResult2 = await hmacManager2.SignAsync(request);
        var verifyResult2 = await hmacManager2.VerifyAsync(request);
        var debug = verifyResult2;
    }
}