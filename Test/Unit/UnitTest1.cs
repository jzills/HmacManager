using System.Runtime.Caching;
using HmacManagement.Caching;
using HmacManagement.Caching.Memory;
using HmacManagement.Components;
using HmacManagement.Mvc.Extensions;
using HmacManagement.Policies;
using HmacManagement.Remodel;
using Microsoft.Extensions.DependencyInjection;

namespace Unit;

public class Tests
{
    public readonly IServiceProvider ServiceProvider;

    public Tests()
    {
        var services = new ServiceCollection();
        services.AddAuthentication()
            .AddHmac(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.Keys.PublicKey = Guid.Parse("4c59aec6-517c-47b0-a681-3c0251037416");
                    policy.Keys.PrivateKey = "CKnebrN5WUmFdIZE01O3hA==";
                    policy.AddHeaderScheme("Scheme1", scheme =>
                    {
                        scheme.AddRequiredHeader("X-Email");
                        scheme.AddRequiredHeader("X-AccountId");
                    });
                });
            });
    
        ServiceProvider = services.BuildServiceProvider();
    }

    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public async Task Sign_Get_Request_With_Single_Query_Parameter()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/accounts?id=1");
        var manager = ServiceProvider.GetRequiredService<IHmacManager>();
        var signingResult = await manager.SignAsync(request);
        
        Assert.NotNull(signingResult.Hmac);
        Assert.True(signingResult.IsSuccess);
        Assert.True(!signingResult.Hmac!.SignedHeaders!.Any());

        Assert.Pass();
    }
}