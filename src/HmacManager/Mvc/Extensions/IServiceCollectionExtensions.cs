using Microsoft.Extensions.DependencyInjection;
using HmacManager.Mvc.Extensions.Internal;
using Microsoft.Extensions.Configuration;

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
    /// <returns>An <c>IServiceCollection</c> that can be used to further configure services.</returns>
    public static IServiceCollection AddHmacManager(
        this IServiceCollection services, 
        Action<HmacManagerOptions> configureOptions
    )
    {
        var options = new HmacManagerOptions();
        configureOptions.Invoke(options);
        
        return services.AddHmacManager(options);
    }

    /// <summary>
    /// Registers the necessary dependencies to use <c>IHmacManagerFactory</c>
    /// in the dependency injection container with the corresponding <c>IConfiguration</c> settings.
    /// </summary>
    /// <param name="services">The <c>IServiceCollection</c>.</param>
    /// <param name="configureOptions">The configuration action for <c>HmacManagerOptions</c>.</param>
    /// <returns>An <c>IServiceCollection</c> that can be used to further configure services.</returns>
    public static IServiceCollection AddHmacManager(
        this IServiceCollection services, 
        IConfiguration configuration
    )
    {
        var options = new HmacManagerOptions();
        configuration.GetSection("HmacManager");//.GetValue();
        // TODO
        //configuration.Bind()
        //configureOptions.Invoke(options);
        
        return services.AddHmacManager(options);
    }
}