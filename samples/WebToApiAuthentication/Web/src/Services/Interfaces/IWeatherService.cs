using Web.Models;

namespace Web.Services;

public interface IWeatherService
{
    Task<IAsyncEnumerable<WeatherForecast?>> GetWeatherForecastAsync();
    Task<WeatherForecastPost?> CreateWeatherForecastAsync();
}