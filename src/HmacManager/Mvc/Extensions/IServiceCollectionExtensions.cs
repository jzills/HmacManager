using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using HmacManager.Mvc.Extensions.Internal;
using HmacManager.Policies;
using HmacManager.Mvc.Configuration;

namespace HmacManager.Mvc.Extensions;

/// <summary>
/// A class representing extension methods on an <c>IServiceCollection</c>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers the necessary dependencies to use <c>IHmacManagerFactory</c>
    /// in the dependency injection container with the configured <c>HmacManagerOptions</c>.
    ///     <para>
    ///         See <see href="https://github.com/jzills/HmacManager/tree/main/samples/">here</see> for examples.
    ///     </para>
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
    ///     <para>
    ///         See <see href="https://github.com/jzills/HmacManager/tree/main/samples/">here</see> for examples.
    ///     </para>
    /// </summary>
    /// <param name="services">The <c>IServiceCollection</c>.</param>
    /// <param name="configurationSection">The <c>IConfigurationSection</c> for an array of JSON objects representing an <c>HmacPolicy</c>.</param>
    /// <returns>An <c>IServiceCollection</c> that can be used to further configure services.</returns>
    public static IServiceCollection AddHmacManager(
        this IServiceCollection services, 
        IConfigurationSection configurationSection
    )
    {
        var policiesToBuild = configurationSection.Get<List<HmacPolicyConfigurationSection>>();
        if (policiesToBuild?.Count > 0)
        {
            var policies = new HmacPolicyCollection();
            foreach (var policy in policiesToBuild)
            {
                var builder = new HmacPolicyConfigurationBuilder(policy);
                policies.Add(builder.Build(policy.Name));
            }

            return services.AddHmacManager(new HmacManagerOptions(policies));
        }
        else
        {
            throw new ConfigurationErrorsException($"No policies could be bound from the configuration section: \"{configurationSection.Key}\"");
        }
    }
}