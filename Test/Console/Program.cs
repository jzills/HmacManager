using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HmacManagement.Components;
using HmacManagement.Mvc.Extensions;
using Microsoft.Extensions.DependencyInjection;

var clientId = "b9926638-6b5c-4a79-a6ca-014d8b848172";
var clientSecret = "11Nv/n22OqU59f9376E//I2rA2+Yg6yRaI0W6YRK/G0=";

var serviceProvider = new ServiceCollection()
    .AddMemoryCache()
    .AddHmacManagement(options =>
    {
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.SignedHeaders = new string[] { "X-AccountId", "X-Email" };
    }).BuildServiceProvider();

var hmacManager = serviceProvider.GetRequiredService<IHmacManager>();
var request = new HttpRequestMessage(HttpMethod.Get, "api/Groups/GetGroups")
{
    Content = new StringContent(
        JsonSerializer.Serialize(new 
        { 
            AccountId = 1, 
            UserId = 2 
        }), 
        new MediaTypeHeaderValue("application/json", Encoding.UTF8.WebName))
};

var signingResult = await hmacManager.SignAsync(request, new Header[]
{
    new Header { Name = "X-AccountId", Value = null },
    new Header { Name = "X-Email", Value = "Joshua.Zillwood@trustedtech.com" }
});

var debug = signingResult;

var verificationResult = await hmacManager.VerifyAsync(request);
debug = verificationResult;