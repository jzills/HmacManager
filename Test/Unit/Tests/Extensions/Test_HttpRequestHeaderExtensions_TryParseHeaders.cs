using HmacManager.Components;
using HmacManager.Extensions;
using HmacManager.Headers;
using HmacManager.Mvc.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Tests;

public class Test_HttpRequestHeaderExtensions_TryParseHeaders : TestBase
{
    [Test]
    public async Task Test()
    {
        var services = new ServiceCollection()
            .AddMemoryCache() 
            // TODO: Handle error when AddMemoryCache isn't called
            // and HmacManager attempts to use IMemoryCache through UseInMemoryCache call.
            .AddHmacManager(options =>
            {
                options.AddPolicy("MyPolicy_1", policy =>
                {
                    policy.UsePublicKey(PublicKey);
                    policy.UsePrivateKey(PrivateKey);
                    policy.UseInMemoryCache(TimeSpan.FromSeconds(30));
                    policy.AddScheme("MyScheme_1", scheme =>
                    {
                        scheme.AddHeader("X-Account-Id");
                        scheme.AddHeader("X-Email");
                    });
                });
            });

        var serviceProvider = services.BuildServiceProvider();
        var hmacManagerFactory = serviceProvider.GetRequiredService<IHmacManagerFactory>();

        // Read request, parse auth header
        var hmacManager1 = hmacManagerFactory.Create("MyPolicy_1");

        var headerScheme = new HeaderScheme("MyScheme_1");
        headerScheme.AddHeader("X-Account-Id");
        headerScheme.AddHeader("X-Email");

        var request = new HttpRequestMessage(HttpMethod.Get, "api/endpoint");
        request.Headers.Add("X-Account-Id", Guid.NewGuid().ToString());
        var hasHeaderValues = request.Headers.TryParseHeaders(headerScheme, out var headerValues);
        Assert.IsFalse(hasHeaderValues);
    }
}