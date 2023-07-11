using System.Net.Http;
using HmacManager.Components;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly HttpClient _client;
    private readonly IHmacManager _hmacManager;

    public WeatherForecastController(
        HttpClient client,
        IHmacManager hmacManager
    ) => (_client, _hmacManager) = (client, hmacManager);

    [HttpGet]
    public async Task<IActionResult> Get(string email)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7058/weatherforecast?email={email}");
        var signingResult = await _hmacManager.SignAsync(request);
        
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        return Ok(responseContent);
    }
}
