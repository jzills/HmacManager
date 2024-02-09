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

    public HomeController(IHttpClientFactory clientFactory) => 
        _client = clientFactory.CreateClient("Hmac");

    public async Task<IActionResult> Index()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7216/weatherforecast");
        requestMessage.Headers.Add("X-Account-Id", Guid.NewGuid().ToString());
        requestMessage.Headers.Add("X-Email", "someuser@email.com");
        var response = await _client.SendAsync(requestMessage);
        return View();
    }
}
