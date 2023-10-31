using System.Diagnostics;
using HmacManagement.Components;
using HmacManagement.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly IHmacManagerFactory _hmacFactory;

    public HomeController(IHmacManagerFactory hmacFactory) => _hmacFactory = hmacFactory;

    public async Task<IActionResult> Index()
    {
        var _hmacManager = _hmacFactory.Create("MyFirstPolicy", "AccountEmailScheme");
        var requestMessage = HttpContext.GetHttpRequestMessage();
        requestMessage.Headers.Add("X-Account-Id", Guid.NewGuid().ToString());
        requestMessage.Headers.Add("X-Email", "joshzillwood@gmail.com");
        var signingResult = await _hmacManager.SignAsync(requestMessage);
        var verificationResult = await _hmacManager.VerifyAsync(HttpContext.GetHttpRequestMessage());
        return View();
    }
}
