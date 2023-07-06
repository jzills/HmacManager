using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
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
    .AddHMACManager(options =>
    {
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.MaxAge = TimeSpan.FromSeconds(30);
        options.MessageContentHeaders = new string[] { "X-User-Id" };
    });

var serviceProvider = services.BuildServiceProvider();
var hmacManager = serviceProvider.GetRequiredService<IHMACManager>();

var signResult = await hmacManager.SignAsync(request, new MessageContent[]
{
    new() { Header = "X-User-Id", Value = Guid.NewGuid().ToString() }
});

var verificationResult  = await hmacManager.VerifyAsync(request);
var isTrusted = verificationResult.IsTrusted;
var verificationResult2 = await hmacManager.VerifyAsync(request);
var checkAgain = verificationResult2.IsTrusted;
var debug = checkAgain;