using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HmacManager.Components;

namespace HmacManager.Mvc;

public class HmacEvents
{
    public Func<HttpContext, HmacResult, Claim[]>? OnAuthenticationSuccess { get; set; }
    public Func<HttpContext, HmacResult, Exception>? OnAuthenticationFailure { get; set; }
}