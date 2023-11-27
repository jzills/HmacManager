using Microsoft.Extensions.DependencyInjection;
using HmacManagement.Mvc.Extensions;
using HmacManagement.Components;

namespace Unit.Tests;

public class TestServiceCollection : TestBase
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IHmacManagerFactory HmacManagerFactory;

    public TestServiceCollection()
    {
        var services = new ServiceCollection()
            .AddMemoryCache()
            .AddStackExchangeRedisCache(options => options.Configuration = "127.0.0.1:6379");

        services
            .AddAuthentication()
            .AddHmac(options =>
            {
                options.AddPolicy(PolicySchemeType.Policy_InMemory.Policy, policy =>
                {
                    policy.UsePublicKey(PublicKey);
                    policy.UsePrivateKey(PrivateKey);
                    policy.UseContentHashAlgorithm(ContentHashAlgorithm.SHA256);
                    policy.UseSigningHashAlgorithm(SigningHashAlgorithm.HMACSHA256);
                    policy.UseInMemoryCache(TimeSpan.FromMinutes(5));

                    policy.AddScheme(PolicySchemeType.Policy_InMemory_Scheme_1.Scheme, scheme =>
                    {
                        scheme.AddHeader("Scheme_Header_1");
                    });

                    policy.AddScheme(PolicySchemeType.Policy_InMemory_Scheme_2.Scheme, scheme =>
                    {
                        scheme.AddHeader("Scheme_Header_1", "Scheme_Header_1_ClaimType");
                        scheme.AddHeader("Scheme_Header_2", "Scheme_Header_2_ClaimType");
                    });

                    policy.AddScheme(PolicySchemeType.Policy_InMemory_Scheme_3.Scheme, scheme =>
                    {
                        scheme.AddHeader("Scheme_Header_1");
                        scheme.AddHeader("Scheme_Header_2", "Scheme_Header_2_ClaimType");
                        scheme.AddHeader("Scheme_Header_3", "Scheme_Header_3_ClaimType");
                    });
                });

                options.AddPolicy(PolicySchemeType.Policy_Distributed.Policy, policy =>
                {
                    policy.UsePublicKey(PublicKey);
                    policy.UsePrivateKey(PrivateKey);
                    policy.UseContentHashAlgorithm(ContentHashAlgorithm.SHA256);
                    policy.UseSigningHashAlgorithm(SigningHashAlgorithm.HMACSHA256);
                    policy.UseDistributedCache(TimeSpan.FromMinutes(5));

                    policy.AddScheme(PolicySchemeType.Policy_Distributed_Scheme_1.Scheme, scheme =>
                    {
                        scheme.AddHeader("Scheme_Header_1");
                    });

                    policy.AddScheme(PolicySchemeType.Policy_Distributed_Scheme_2.Scheme, scheme =>
                    {
                        scheme.AddHeader("Scheme_Header_1", "Scheme_Header_1_ClaimType");
                        scheme.AddHeader("Scheme_Header_2", "Scheme_Header_2_ClaimType");
                    });

                    policy.AddScheme(PolicySchemeType.Policy_Distributed_Scheme_3.Scheme, scheme =>
                    {
                        scheme.AddHeader("Scheme_Header_1");
                        scheme.AddHeader("Scheme_Header_2", "Scheme_Header_2_ClaimType");
                        scheme.AddHeader("Scheme_Header_3", "Scheme_Header_3_ClaimType");
                    });
                });
            });

        ServiceProvider = services.BuildServiceProvider();
        HmacManagerFactory = ServiceProvider.GetRequiredService<IHmacManagerFactory>();
    }
}