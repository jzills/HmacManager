using HmacManagement.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet(Name = "GetWeatherForecast")]
    [Authorize(AuthenticationSchemes = HmacAuthenticationDefaults.Scheme)]
    public IActionResult Get(string email) => Ok(email);
}
