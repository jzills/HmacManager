using System.Net.Http;
using HmacManager.Components;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IMyClient _client;
    private readonly IHmacManager _hmacManager;

    public WeatherForecastController(
        IMyClient client,
        IHmacManager hmacManager
    ) => (_client, _hmacManager) = (client, hmacManager);

    [HttpGet]
    public async Task<IActionResult> Get(string email)
    {
        await _client.SendAsync($"weatherforecast?email={email}");
        return Ok("done");
    }
}
