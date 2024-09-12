using Web.Models;
using System.Text;
using System.Text.Json;

namespace Web.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _client;

    // Create an instance of the "Hmac_MyPolicy_RequireAccountAndEmail"
    // HttpClient which is registered in the Program.cs
    public WeatherService(IHttpClientFactory clientFactory) => 
        _client = clientFactory.CreateClient("Hmac_MyPolicy_RequireAccountAndEmail");

    public async Task<IAsyncEnumerable<WeatherForecast?>> GetWeatherForecastAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/weatherforecast");

        // Add the required headers for the policy scheme combination
        // from the requested client in the constructor above
        request.Headers.Add("X-Account", "123");
        request.Headers.Add("X-Email", "my@email.com");

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
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/weatherforecast")
        {
            Content = new StringContent(JsonSerializer.Serialize(new WeatherForecastPost
            {
                Summary = "This is a test."
            }), Encoding.UTF8, "application/json")
        };

        // Add the required headers for the policy scheme combination
        // from the requested client in the constructor above
        request.Headers.Add("X-Account", "123");
        request.Headers.Add("X-Email", "my@email.com");

        var response = await _client.SendAsync(request);
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