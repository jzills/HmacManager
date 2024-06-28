using System.Configuration;
using Microsoft.Extensions.Configuration;
using HmacManager.Mvc.Configuration;
using HmacManager.Policies;

namespace HmacManager.Mvc.Extensions.Internal;

internal static class IConfigurationSectionExtensions
{
    public static HmacPolicyCollection GetPolicySection(this IConfigurationSection configurationSection)
    {
        var policiesToBuild = configurationSection.Get<List<HmacPolicyConfigurationSection>>();
        if (policiesToBuild?.Count > 0)
        {
            var policies = new HmacPolicyCollection();
            foreach (var policy in policiesToBuild)
            {
                var configBuilder = new HmacPolicyConfigurationBuilder(policy);
                policies.Add(configBuilder.Build(policy.Name));
            }

            return policies;
        }
        else
        {
            throw new ConfigurationErrorsException($"No policies could be bound from the configuration section: \"{configurationSection.Key}\"");
        }
    }
}