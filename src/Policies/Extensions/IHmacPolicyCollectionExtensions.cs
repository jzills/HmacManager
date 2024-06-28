using HmacManager.Common;
using HmacManager.Common.Extensions;

namespace HmacManager.Policies.Extensions;

/// <summary>
/// A class representing extensions for <c>IHmacPolicyCollection</c>.
/// </summary>
public static class IHmacPolicyCollectionExtensions
{
    /// <summary>
    /// Attempts to retrieve an <c>HmacPolicy</c> based on the specified name.
    /// </summary>
    /// <param name="source">An <c>IHmacPolicyCollection</c> implementation instance.</param>
    /// <param name="name">A <c>string</c> name representing an <c>HmacPolicy</c>.</param>
    /// <param name="policy">An <c>HmacPolicy</c>.</param>
    /// <returns>A <c>bool</c> indicating the success of the operation.</returns>
    public static bool TryGetValue(this IHmacPolicyCollection source, string name, out HmacPolicy policy)
    {
        if (source is IComponentCollection<HmacPolicy> components)
        {
            return components.TryGetValue(name, out policy);
        }
        else
        {
            policy = default!;
            return false;
        }
    }
}