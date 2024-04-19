using System.Net.Http.Headers;
using System.Text.Json;

namespace Unit.Tests;

public class TestCaseSource
{
    public static IEnumerable<HttpRequestMessage> GetHttpRequestMessages() =>
        [
            new HttpRequestMessage(HttpMethod.Get,    "api/artists"),
            new HttpRequestMessage(HttpMethod.Get,   "/api/artists"),
            new HttpRequestMessage(HttpMethod.Get,  $"/api/artists/{Guid.NewGuid()}"),
            new HttpRequestMessage(HttpMethod.Post, $"/api/artists/")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new
                    {
                        Id = Guid.NewGuid(),
                        Artist = "John Coltrane",
                        Genre = "Jazz"
                    }), 
                    new MediaTypeHeaderValue("application/json")
                )
            }
        ];
}