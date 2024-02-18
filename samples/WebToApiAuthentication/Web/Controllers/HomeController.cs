using Api;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _client;

    public HomeController(IHttpClientFactory clientFactory) => 
        _client = clientFactory.CreateClient("Hmac_MyPolicy_RequiredAccountAndEmail");

    public IActionResult Index() => View();

    public async Task<IActionResult> GetWeatherForecast()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/weatherforecast");
        request.Headers.Add("X-Account", Guid.NewGuid().ToString());
        request.Headers.Add("X-Email", "my@email.com");

        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return Ok(response.Content.ReadFromJsonAsAsyncEnumerable<WeatherForecast>());
        }
        else
        {
            return BadRequest();
        }
    }
}
