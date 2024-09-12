using Microsoft.AspNetCore.Mvc;
using HmacManager.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[HmacAuthenticate(Policy = "MyPolicy", Scheme = "RequireAccountAndEmail")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = [
        "Freezing", 
        "Bracing", 
        "Chilly", 
        "Cool", 
        "Mild", 
        "Warm", 
        "Balmy", 
        "Hot", 
        "Sweltering", 
        "Scorching"
    ];

    [HttpGet]
    [Route("")]
    public IEnumerable<WeatherForecast> Get() =>
        Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
}
