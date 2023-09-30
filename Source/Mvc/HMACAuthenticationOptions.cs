using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace HmacManagement.Mvc;

public class HmacEvents
{
    public Func<HttpContext, Claim[]>? OnAuthenticationSuccess { get; set; }
}

public class HmacAuthenticationOptions : AuthenticationSchemeOptions
{
    public new HmacEvents Events { get; set; } = default!;
}