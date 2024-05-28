using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using HmacManager.Mvc.Extensions.Internal;
using HmacManager.Policies;
using HmacManager.Headers;

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
    /// <param name="configurationSection">The <c>IConfigurationSection</c> for an array of JSON objects representing an <c>HmacPolicy</c>.</param>
    /// <returns>An <c>IServiceCollection</c> that can be used to further configure services.</returns>
    public static IServiceCollection AddHmacManager(
        this IServiceCollection services, 
        IConfigurationSection configurationSection
    )
    {
        var policiesToValidate = new List<HmacPolicyJsonConfiguration>();
        configurationSection.Bind(policiesToValidate);

        var policies = new HmacPolicyCollection();
        foreach (var policy in policiesToValidate)
        {
            var headerSchemes = new HeaderSchemeCollection();
            if (policy?.HeaderSchemes?.Any() ?? false)
            {
                foreach (var headerScheme in policy.HeaderSchemes)
                {
                    var scheme = new HeaderScheme(headerScheme.Name);
                    if (headerScheme?.Headers?.Any() ?? false)
                    {
                        foreach (var header in headerScheme.Headers)
                        {
                            scheme.AddHeader(header.Name, header.ClaimType);
                        }
                    }

                    headerSchemes.Add(scheme);
                }
            }

            policies.Add(new HmacPolicy(policy.Name)
            {
                Algorithms = policy.Algorithms,
                Keys = policy.Keys,
                Nonce = policy.Nonce,
                HeaderSchemes = headerSchemes
            });
        }

        return services.AddHmacManager(new HmacManagerOptions(policies));
    }
}