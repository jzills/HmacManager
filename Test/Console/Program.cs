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
        // options.HeaderSchemes.Add("MySecondScheme", headers =>
        // {
        //     headers.Add("MySecondRequiredHeader1");
        //     headers.Add("MySecondRequiredHeader2");
        // });
    }).BuildServiceProvider();

var hmacManager = serviceProvider.GetRequiredService<IHmacManager>();
var request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://www.myapi.com/endpoint?id=1"));
var signingResult = await HmacManagement.SignAsync(request);
var debug = signingResult;