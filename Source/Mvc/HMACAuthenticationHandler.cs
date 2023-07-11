// using System.Security.Claims;
// using System.Text.Encodings.Web;
// using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using Souce.Mvc;
// using HmacManager.Components;

// namespace HmacManager.Mvc;

// public class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
// {
//     private readonly IHmacManager _HmacManager;

//     public HmacAuthenticationHandler(
//         IHmacManager HmacManager,
//         IOptionsMonitor<HmacAuthenticationOptions> options, 
//         ILoggerFactory logger, 
//         UrlEncoder encoder, 
//         ISystemClock clock
//     ) : base(options, logger, encoder, clock)
//     {
//         _HmacManager = HmacManager;
//     }

//     protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
//     {
//         var HmacResult = await _HmacManager.VerifyAsync(
//             Request.HttpContext.GetHttpRequestMessage());
            
//         if (HmacResult.IsSuccess)
//         {
//             return AuthenticateResult.Success(
//                 new AuthenticationTicket(
//                     new ClaimsPrincipal(), 
//                     new AuthenticationProperties(), 
//                     HmacAuthenticationDefaults.Scheme));
//         }
//         else
//         {
//             return AuthenticateResult.Fail(new Exception());
//         }
//     }
// }