using System.Configuration;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using HmacManager.Policies;
using HmacManager.Policies.Extensions;

namespace HmacManager.Mvc.Extensions.Internal;

/// <summary>
/// Watches an <see cref="IConfigurationSection"/> for changes and synchronizes an
/// <see cref="IHmacPolicyCollection"/> in place, so edits to the underlying configuration
/// (e.g. a rotated key delivered via a reloadable mounted ConfigMap/Secret) take effect
/// without restarting the host process.
/// </summary>
internal sealed class HmacPolicyCollectionReloader
{
    private readonly IConfigurationSection ConfigurationSection;
    private readonly IHmacPolicyCollection Policies;

    /// <summary>
    /// Creates an <see cref="HmacPolicyCollectionReloader"/> and immediately subscribes
    /// to configuration reload notifications for the lifetime of the process.
    /// </summary>
    /// <param name="configurationSection">The <see cref="IConfigurationSection"/> policies are bound from.</param>
    /// <param name="policies">The <see cref="IHmacPolicyCollection"/> to keep in sync, already populated with the initial policy set.</param>
    public HmacPolicyCollectionReloader(IConfigurationSection configurationSection, IHmacPolicyCollection policies)
    {
        ConfigurationSection = configurationSection;
        Policies = policies;

        ChangeToken.OnChange(ConfigurationSection.GetReloadToken, Reload);
    }

    /// <summary>
    /// Rebuilds the policy set from the current configuration and synchronizes it into
    /// <see cref="Policies"/> in place: policies no longer present are removed, new or
    /// changed policies are added, and unchanged policies are left untouched.
    /// </summary>
    /// <remarks>
    /// Configuration that fails to bind is ignored and the previous, still-valid policy
    /// set is left in place until the next change notification. This can happen transiently
    /// when public policy fields and private keys are delivered via independently-updated
    /// mounted volumes (ConfigMap vs. Secret), since the two are not guaranteed to update atomically together.
    /// </remarks>
    internal void Reload()
    {
        HmacPolicyCollection updated;
        try
        {
            updated = ConfigurationSection.GetPolicySection();
        }
        catch (ConfigurationErrorsException)
        {
            return;
        }

        var updatedNames = new HashSet<string>(updated.Values.Select(policy => policy.Name!));
        foreach (var existing in Policies.Values.ToList())
        {
            if (!updatedNames.Contains(existing.Name!))
            {
                Policies.Remove(existing.Name!);
            }
        }

        foreach (var policy in updated.Values)
        {
            if (Policies.TryGetValue(policy.Name!, out var current) && AreEqual(current, policy))
            {
                continue;
            }

            Policies.Remove(policy.Name!);
            Policies.Add(policy);
        }
    }

    private static bool AreEqual(HmacPolicy left, HmacPolicy right) =>
        JsonSerializer.Serialize(left) == JsonSerializer.Serialize(right);
}
