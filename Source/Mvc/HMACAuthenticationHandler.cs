using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Souce.Mvc;
using Source.Components;

namespace Source.Mvc;

public class HMACAuthenticationHandler : AuthenticationHandler<HMACAuthenticationOptions>
{
    private readonly IHMACManager _manager;

    public HMACAuthenticationHandler(
        IHMACManager manager,
        IOptionsMonitor<HMACAuthenticationOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
        _manager = manager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var verificationResult = await _manager.VerifyAsync(
            Request.HttpContext.GetHttpRequestMessage());
            
        if (verificationResult.IsTrusted)
        {
            return AuthenticateResult.Success(
                new AuthenticationTicket(
                    new ClaimsPrincipal(), 
                    new AuthenticationProperties(), 
                    HMACAuthenticationDefaults.Scheme));
        }
        else
        {
            return AuthenticateResult.Fail(new Exception());
        }
    }
}