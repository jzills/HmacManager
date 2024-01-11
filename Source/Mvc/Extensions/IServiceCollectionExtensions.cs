using Microsoft.Extensions.DependencyInjection;
using HmacManager.Mvc.Extensions.Internal;
using HmacManager.Extensions;

namespace HmacManager.Mvc.Extensions;

/// <summary>
/// A class representing extension methods on an <c>IServiceCollection</c>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers the necessary dependencies to use <c>IHmacManagerFactory</c>
    /// in the dependency injection container with the configured <c>HmacManagerOptions</c>.
    /// </summary>
    /// <param name="services">The <c>IServiceCollection</c>.</param>
    /// <param name="configureOptions">The configuration action for <c>HmacManagerOptions</c>.</param>
    /// <returns>A <c>IServiceCollection</c>.</returns>
    public static IServiceCollection AddHmacManager(
        this IServiceCollection services, 
        Action<HmacManagerOptions> configureOptions
    )
    {
        var options = new HmacManagerOptions();
        configureOptions.Invoke(options);
        
        return services.AddHmacManager(options);
    }

    // Is this necessary or useful?
    // The extension for AddHmacHttpMessageHandler
    // is most likely sufficient...
    // public static IHttpClientBuilder AddHmacHttpClient(
    //     this IServiceCollection services,
    //     string name,
    //     string policy,
    //     Action<HttpClient> configureClient
    // ) => services.AddHttpClient(name, configureClient)
    //         .AddHmacHttpMessageHandler(policy);

    // public static IHttpClientBuilder AddHmacHttpClient(
    //     this IServiceCollection services,
    //     string name,
    //     string policy,
    //     string scheme,
    //     Action<HttpClient> configureClient
    // ) => services.AddHttpClient(name, configureClient)
    //         .AddHmacHttpMessageHandler(policy, scheme);
}