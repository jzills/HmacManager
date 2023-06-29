using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Source.Caching.Memory;
using Source.Components;

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

var serviceProvider = new ServiceCollection()
    .AddMemoryCache()
    .BuildServiceProvider();

var managerOptions = new HMACManagerOptions
{
    MaxAge = TimeSpan.FromSeconds(5)
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

var hmacManager = new HMACManager(managerOptions, nonceCache, provider);

await hmacManager.SignAsync(request);
var isTrusted  = await hmacManager.VerifyAsync(request);
var checkAgain = await hmacManager.VerifyAsync(request);
var debug = checkAgain;