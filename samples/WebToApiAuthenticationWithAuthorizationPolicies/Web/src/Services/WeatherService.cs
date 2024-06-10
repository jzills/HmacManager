using Web.Models;

namespace Web.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _client;

    public WeatherService(IHttpClientFactory clientFactory) => 
        _client = clientFactory.CreateClient("HmacPolicy_2_HmacScheme_2");

    public async Task<IAsyncEnumerable<WeatherForecast?>> GetWeatherForecastAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/weatherforecast");

        // Add the required headers for the policy scheme combination
        // from the requested client in the constructor above
        request.Headers.Add("X-Scheme_1", "Scheme_1_Value");
        request.Headers.Add("X-Scheme_2", "Scheme_2_Value");

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
}