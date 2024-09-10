using HmacManager.Components;
using HmacManager.Policies;
using HmacManager.Policies.Extensions;
using Microsoft.AspNetCore.Http;

namespace HmacManager.Mvc.Extensions.Internal;

internal static class IHeaderDictionaryExtensions
{
    internal static bool TryGetSignature(this IHeaderDictionary headers, out string signature)
    {
        if (headers.TryGetValue("Authorization", out var hmacAuthorizationHeader))
        {
            if (hmacAuthorizationHeader.Count == 1)
            {
                var hmacAuthorizationHeaderValues = hmacAuthorizationHeader.First()?.Split(" ");
                if (hmacAuthorizationHeaderValues?.Count() == 2)
                {
                    signature = hmacAuthorizationHeaderValues[1];
                    return true;
                }
            }
        }

        signature = default!;
        return false;
    }

    internal static bool TryGetHmacManager(this IHeaderDictionary headers, 
        IHmacManagerFactory hmacManagerFactory, 
        IHmacPolicyCollection hmacPolicies, 
        out IHmacManager? hmacManager, 
        out HmacPolicy? hmacPolicy
    )
    {
        var hasConfiguredPolicy = headers.TryGetNonEmptyValue(HmacAuthenticationDefaults.Headers.Policy, out var policy);
        var hasConfiguredScheme = headers.TryGetNonEmptyValue(HmacAuthenticationDefaults.Headers.Scheme, out var scheme);

        hmacManager = (hasConfiguredPolicy, hasConfiguredScheme) switch
        {
            (true, true)    => hmacManagerFactory.Create(policy, scheme),
            (true, false)   => hmacManagerFactory.Create(policy),
            _               => default
        };

        return hmacPolicies.TryGetValue(policy, out hmacPolicy);
    }

    internal static bool TryGetNonEmptyValue(this IHeaderDictionary headers, 
        string key, 
        out string value
    )
    {
        if (headers.TryGetValue(key, out var values) && values.Count > 0)
        {
            value = values.FirstOrDefault() ?? string.Empty;
            return !string.IsNullOrWhiteSpace(value);
        }
        else
        {
            value = string.Empty;
            return false;
        }
    }
}