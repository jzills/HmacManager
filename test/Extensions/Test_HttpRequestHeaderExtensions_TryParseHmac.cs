using HmacManager.Components;
using HmacManager.Extensions;
using HmacManager.Headers;
using HmacManager.Mvc.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Tests;

public class Test_HttpRequestHeaderExtensions_TryParseHmac : TestBase
{
    [Test]
    public async Task Test()
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
                    policy.AddScheme("MyScheme_1", scheme =>
                    {
                        scheme.AddHeader("X-Account-Id");
                        scheme.AddHeader("X-Email");
                    });
                });

                options.AddPolicy("MyPolicy_2", policy =>
                {
                    policy.UsePublicKey(PublicKey);
                    policy.UsePrivateKey(PrivateKey);
                    policy.UseMemoryCache(TimeSpan.FromSeconds(30));
                    policy.AddScheme("MyScheme_2", scheme =>
                    {
                        scheme.AddHeader("X-Email");
                    });
                });
            });

        var serviceProvider = services.BuildServiceProvider();
        var hmacManagerFactory = serviceProvider.GetRequiredService<IHmacManagerFactory>();
        var hmacManager = hmacManagerFactory.Create("MyPolicy_2");

        var headerScheme = new HeaderScheme("MyScheme_1");
        headerScheme.AddHeader("X-Account-Id");
        headerScheme.AddHeader("X-Email");

        var request = new HttpRequestMessage(HttpMethod.Get, "api/endpoint");
        request.Headers.Add("X-Account-Id", Guid.NewGuid().ToString());
        var signingResult = await hmacManager.SignAsync(request);

        // var hasHeaderValues = request.Headers.TryParseHmac(headerScheme, TimeSpan.FromSeconds(30), out var headerValues);
        // Assert.IsFalse(hasHeaderValues);
    }
}