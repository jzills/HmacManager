using Microsoft.Extensions.DependencyInjection;
using HmacManagement.Mvc.Extensions.Internal;

namespace HmacManagement.Mvc.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the necessary dependencies to use IHmacManagerFactory
    /// in the dependency injection container with the configured HmacManagementOptions.
    /// </summary>
    /// <param name="services">The service collection container.</param>
    /// <param name="configureOptions">The configuration action for HmacManagementOptions.</param>
    /// <returns>The service collection container.</returns>
    public static IServiceCollection AddHmacManagement(
        this IServiceCollection services, 
        Action<HmacManagementOptions> configureOptions
    )
    {
        var options = new HmacManagementOptions();
        configureOptions.Invoke(options);
        
        return services.AddHmacManagement(options);
    }
}