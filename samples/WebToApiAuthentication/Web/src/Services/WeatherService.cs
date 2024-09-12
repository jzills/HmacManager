using Web.Models;
using System.Text;
using System.Text.Json;

namespace Web.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _client;

    // Create an instance of the "Hmac_MyPolicy_RequireAccountAndEmail"
    // HttpClient which is registered in the Program.cs
    public WeatherService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("Hmac_MyPolicy_RequireAccountAndEmail");

        // Add the required headers for the policy scheme combination
        // from the requested client in the constructor above
        _client.DefaultRequestHeaders.Add("X-Account", "123");
        _client.DefaultRequestHeaders.Add("X-Email", "my@email.com");
    }

    public async Task<IAsyncEnumerable<WeatherForecast?>> GetWeatherForecastAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/weatherforecast");
        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return response.Content.ReadFromJsonAsAsyncEnumerable<WeatherForecast>();
        }
        else
        {
            return default!;
        }
    }

    public async Task<WeatherForecastPost?> CreateWeatherForecastAsync()
    {
        var response = await _client.PostAsJsonAsync("api/weatherforecast", new WeatherForecastPost
        {
            Summary = "This is a test."
        });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<WeatherForecastPost>();
        }
        else
        {
            return default!;
        }
    }
}