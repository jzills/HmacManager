using Web.Models;

namespace Web.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _client;

    // Create an instance of the "Hmac_MyPolicy_RequireAccountAndEmail"
    // HttpClient which is registered in the Program.cs
    public WeatherService(IHttpClientFactory clientFactory) => 
        _client = clientFactory.CreateClient("Hmac_Some_PolicyScheme");

    public async Task<IAsyncEnumerable<WeatherForecast?>> GetWeatherForecastAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/weatherforecast");

        // Add the required headers for the policy scheme combination
        // from the requested client in the constructor above
        request.Headers.Add("Some_Header_1", "Some_Value_1");
        request.Headers.Add("Some_Header_2", "Some_Value_2");
        request.Headers.Add("Some_Header_3", "Some_Value_3");

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