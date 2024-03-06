using System.Security.Claims;
using HmacManager.Components;
using Microsoft.AspNetCore.Http;

namespace HmacManager.Mvc;

public class HmacEvents
{
    public Func<HttpContext, HmacResult, Claim[]>? OnAuthenticationSuccess { get; set; }
    public Func<HttpContext, Exception>? OnAuthenticationFailure { get; set; }
}