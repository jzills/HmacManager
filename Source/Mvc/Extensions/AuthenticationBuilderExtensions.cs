using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using HmacManagement.Caching;
using HmacManagement.Components;
using HmacManagement.Policies;
using Microsoft.Extensions.Caching.Memory;
using HmacManagement.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using HmacManagement.Caching.Distributed;

namespace HmacManagement.Mvc.Extensions;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddHmac(
        this AuthenticationBuilder builder,
        Action<HmacAuthenticationOptions> configureOptions
    )
    {
        builder.AddScheme<HmacAuthenticationOptions, HmacAuthenticationHandler>(
            HmacAuthenticationDefaults.AuthenticationScheme,
            HmacAuthenticationDefaults.AuthenticationScheme,
            configureOptions
        );

        builder.Services.AddHttpContextAccessor();

        var options = new HmacAuthenticationOptions();
        configureOptions.Invoke(options);

        var serviceProvider = builder.Services.BuildServiceProvider();

        // TODO: The caches are not configured
        // based on user configuration. Right now,
        // these values are hard coded. Max age and cache name
        // needs to be configurable to support diferent max ages.
        var caches = new NonceCacheCollection();
        var memoryCache = serviceProvider.GetService<IMemoryCache>();
        if (memoryCache is not null)
        {
            caches.Add("InMemory", new NonceMemoryCache(memoryCache, new NonceCacheOptions
                { MaxAge = TimeSpan.FromMinutes(1) }));
        }

        var distributedCache = serviceProvider.GetService<IDistributedCache>();
        if (distributedCache is not null)
        {
            caches.Add("Distributed", new NonceDistributedCache(distributedCache, new NonceCacheOptions
                { MaxAge = TimeSpan.FromMinutes(1) }));
        }

        builder.Services.AddScoped<IComponentCollection<INonceCache>, NonceCacheCollection>(_ => caches);
        builder.Services.AddScoped<IComponentCollection<HmacPolicy>, HmacPolicyCollection>(_ => options.GetPolicies());
        builder.Services.AddScoped<IHmacManagerFactory, HmacManagerFactory>();
        
        return builder;
    }
}