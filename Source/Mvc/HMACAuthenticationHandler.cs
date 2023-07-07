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
    private readonly IHMACManager _hmacManager;

    public HMACAuthenticationHandler(
        IHMACManager hmacManager,
        IOptionsMonitor<HMACAuthenticationOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
        _hmacManager = hmacManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var hmacResult = await _hmacManager.VerifyAsync(
            Request.HttpContext.GetHttpRequestMessage());
            
        if (hmacResult.IsSuccess)
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