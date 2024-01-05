using System.Diagnostics;
using HmacManager.Components;
using HmacManager.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _client;
    private readonly IHmacManagerFactory _hmacFactory;

    public HomeController(
        HttpClient client,
        IHmacManagerFactory hmacFactory
    ) => (_client, _hmacFactory) = (client, hmacFactory);

    public async Task<IActionResult> Index()
    {
        var _hmacManager = _hmacFactory.Create("MyPolicy_1", "AccountEmailScheme");
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7216/weatherforecast");
        // requestMessage.Headers.Add("X-Account-Id", Guid.NewGuid().ToString());
        requestMessage.Headers.Add("X-Email", "someuser@email.com");
        var signingResult = await _hmacManager.SignAsync(requestMessage);
        var response = await _client.SendAsync(requestMessage);
        return View();
    }
}
