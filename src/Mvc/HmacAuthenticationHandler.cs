using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using HmacManager.Components;
using HmacManager.Exceptions;

namespace HmacManager.Mvc;

internal class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
{
    private readonly IHmacManagerFactory _hmacManagerFactory;

    public HmacAuthenticationHandler(
        IHmacManagerFactory hmacManagerFactory,
        IOptionsMonitor<HmacAuthenticationOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
        _hmacManagerFactory = hmacManagerFactory;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (TryGetSignature(Request.Headers, out var hmacSignature))
        {
            if (TryGetManager(Request.Headers, out var hmacManager))
            {
                var request = Request.HttpContext.GetHttpRequestMessage();
                var hmacResult = await hmacManager!.VerifyAsync(request);
                
                RewindBody(Request.Body);

                if (hmacResult.IsSuccess)
                {
                    var claims = new List<Claim>();
                    foreach (var headerValue in hmacResult!.Hmac!.HeaderValues)
                    {
                        claims.Add(new Claim(
                            headerValue.ClaimType, 
                            headerValue.Value
                        ));
                    }

                    if (Options.Events?.OnAuthenticationSuccess is not null)
                    {
                        var successHandlerClaims = Options.Events.OnAuthenticationSuccess(Request.HttpContext, hmacResult);
                        foreach (var claim in successHandlerClaims)
                        {
                            claims.Add(claim);
                        }
                    }

                    // Add policy and header claim to support IAuthorizationRequirement
                    // for dynamic authorization policies.
                    claims.Add(new Claim(HmacAuthenticationDefaults.Properties.PolicyProperty, hmacResult.Policy));
                    claims.Add(new Claim(HmacAuthenticationDefaults.Properties.SchemeProperty, hmacResult.HeaderScheme));

                    return AuthenticateResult.Success(
                        new AuthenticationTicket(
                            new ClaimsPrincipal(
                                new ClaimsIdentity(claims, 
                                    HmacAuthenticationDefaults.AuthenticationScheme)), 
                            new AuthenticationProperties(new Dictionary<string, string?> 
                            { 
                                { HmacAuthenticationDefaults.Properties.PolicyProperty, hmacResult.Policy }, 
                                { HmacAuthenticationDefaults.Properties.SchemeProperty, hmacResult.HeaderScheme }
                            }), 
                            HmacAuthenticationDefaults.AuthenticationScheme
                        ));
                }
                else
                {
                    if (Options.Events?.OnAuthenticationFailure is not null)
                    {
                        var exception = Options.Events.OnAuthenticationFailure(Request.HttpContext, hmacResult);
                        return AuthenticateResult.Fail(exception);
                    }
                    else
                    {
                        return AuthenticateResult.Fail(new HmacAuthenticationException());
                    }
                }
            }
            else
            {
                return AuthenticateResult.Fail(new MissingHeaderException());
            }
        }
        else
        {
            return AuthenticateResult.NoResult();
        }
    }

    private void RewindBody(Stream body)
    {
        if (body.CanSeek)
        {
            body.Seek(0, SeekOrigin.Begin);
        }
    }

    private bool TryGetSignature(IHeaderDictionary headers, out string signature)
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

    private bool TryGetManager(IHeaderDictionary headers, out IHmacManager? manager)
    {
        var hasConfiguredPolicy = headers.TryGetValue(HmacAuthenticationDefaults.Headers.Policy, out var policy) && !string.IsNullOrWhiteSpace(policy);
        var hasConfiguredScheme = headers.TryGetValue(HmacAuthenticationDefaults.Headers.Scheme, out var scheme) && !string.IsNullOrWhiteSpace(scheme);

        manager = (hasConfiguredPolicy, hasConfiguredScheme) switch
        {
            (true, true)    => _hmacManagerFactory.Create(policy!, scheme!),
            (true, false)   => _hmacManagerFactory.Create(policy!),
            _               => default!
        };

        return hasConfiguredPolicy;
    }
}