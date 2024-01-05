using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HmacManager.Mvc;

public class HmacEvents
{
    public Func<HttpContext, Claim[]>? OnAuthenticationSuccess { get; set; }
    public Func<HttpContext, Exception>? OnAuthenticationFailure { get; set; }
}