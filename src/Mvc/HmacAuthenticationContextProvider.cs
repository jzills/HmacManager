using HmacManager.Components;
using HmacManager.Policies;
using HmacManager.Policies.Extensions;
using Microsoft.AspNetCore.Http;

namespace HmacManager.Mvc;

public interface IHmacAuthenticationContextProvider
{
    bool TryGetAuthenticationContext(IDictionary<string, string> headers, out HmacAuthenticationContext context);
    bool TryGetAuthenticationContext(IHeaderDictionary headers, out HmacAuthenticationContext context);
}

public class HmacAuthenticationContext
{
    public IHmacManager HmacManager;
    public HmacPolicy Policy;
    public string Signature;
}

public class HmacAuthenticationContextProvider : IHmacAuthenticationContextProvider
{
    protected readonly IHmacManagerFactory Factory;

    protected readonly IHmacPolicyCollection Policies;

    protected readonly IHmacHeaderParserFactory HeaderParserFactory;

    public HmacAuthenticationContextProvider(
        IHmacManagerFactory factory,
        IHmacPolicyCollection policies,
        IHmacHeaderParserFactory headerParserFactory
    )
    {
        Factory = factory;
        Policies = policies;
        HeaderParserFactory = headerParserFactory;
    }

    public bool TryGetAuthenticationContext(IDictionary<string, string?> headers, out HmacAuthenticationContext context)
    {
        var hmacPartial = HeaderParserFactory.Create(headers).Parse(out var signature);
        var hmacManager = Factory.Create(hmacPartial.Policy, hmacPartial.HeaderScheme);

        context = new HmacAuthenticationContext
        {
            HmacManager = hmacManager,
            Signature = signature
        };

        if (Policies.TryGetValue(hmacPartial.Policy, out var hmacPolicy))
        {
            context.Policy = hmacPolicy;
        }
        else
        {
            throw new Exception();
        }

        return true;
    }

    public bool TryGetAuthenticationContext(IHeaderDictionary headers, out HmacAuthenticationContext context) =>
        TryGetAuthenticationContext(
            headers.ToDictionary(
                header => header.Key,
                header => header.Value.FirstOrDefault()
        ), out context);
}