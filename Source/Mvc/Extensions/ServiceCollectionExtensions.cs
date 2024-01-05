using Microsoft.Extensions.DependencyInjection;
using HmacManager.Mvc.Extensions.Internal;

namespace HmacManager.Mvc.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the necessary dependencies to use IHmacManagerFactory
    /// in the dependency injection container with the configured HmacManagerOptions.
    /// </summary>
    /// <param name="services">The service collection container.</param>
    /// <param name="configureOptions">The configuration action for HmacManagerOptions.</param>
    /// <returns>The service collection container.</returns>
    public static IServiceCollection AddHmacManager(
        this IServiceCollection services, 
        Action<HmacManagerOptions> configureOptions
    )
    {
        var options = new HmacManagerOptions();
        configureOptions.Invoke(options);
        
        return services.AddHmacManager(options);
    }
}