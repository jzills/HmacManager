using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _client;

    public HomeController(IHttpClientFactory clientFactory) => 
        _client = clientFactory.CreateClient("HmacPolicy_2_HmacScheme_2");

    public IActionResult Index() => View();

    public async Task<IActionResult> GetWeatherForecast()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/weatherforecast");
        request.Headers.Add("X-Scheme_1", "Scheme_1_Value");
        request.Headers.Add("X-Scheme_2", "Scheme_2_Value");

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
