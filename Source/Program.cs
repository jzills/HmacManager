using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Souce.Mvc;
using Source.Caching.Memory;
using Source.Components;
using Source.Mvc.Extensions;

var clientId = Guid.NewGuid().ToString();
var clientSecret = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())));

var request = new HttpRequestMessage(HttpMethod.Post, "https://www.someUri.com")
{
    Content = new StringContent(JsonSerializer.Serialize(new
    {
        Id = 1,
        Name = "Joshua"
    }))
};

var services = new ServiceCollection();
services
    .AddMemoryCache()
    // .AddHMACManager()
    .AddAuthentication()
    .AddHMAC(options =>
    {
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.MaxAge = TimeSpan.FromSeconds(30);
        options.AdditionalContentHeaders = new string[] { "X-Person-Id" };
    });

var serviceProvider = services.BuildServiceProvider();

var managerOptions = new HMACManagerOptions
{
    MaxAge = TimeSpan.FromSeconds(5),
    AdditionalContentHeaders = new string[] { "X-Person-Id" }
};

var nonceCache = new NonceMemoryCache(
    serviceProvider.GetRequiredService<IMemoryCache>(), 
    managerOptions
);

var provider = new HMACProvider(new HMACProviderOptions
{
    ClientId = clientId,
    ClientSecret = clientSecret
});

var hmacManager = serviceProvider.GetRequiredService<IHMACManager>();//new HMACManager(managerOptions, nonceCache, provider);

var signResult = await hmacManager.SignAsync(request, new MessageContent[]
{
    new MessageContent 
    { 
        Header = "X-Person-Id", 
        Value = Guid.NewGuid().ToString() 
    }
});

var isTrusted  = await hmacManager.VerifyAsync(request);
var checkAgain = await hmacManager.VerifyAsync(request);
var debug = checkAgain;