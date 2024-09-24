using HmacManager.Components;
using HmacManager.Mvc;
using HmacManager.Mvc.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

[TestFixture]
public class HttpContextRequestBodyDynamicTest
{
    private TestServer _server;
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        // Dynamically configure the WebHostBuilder
        var builder = new WebHostBuilder()
            .ConfigureServices(services => 
            {
                services.AddRouting();
                services.AddMemoryCache();
                services.AddAuthentication().AddHmac(options =>
                {
                    // The same setup as in the Web project for this demo
                    options.AddPolicy("MyPolicy", policy =>
                    {
                        policy.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
                        policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolPrivateKey")));
                        policy.UseMemoryCache(30);
                        policy.AddScheme("RequireAccountAndEmail", scheme =>
                        {
                            scheme.AddHeader("X-Account");
                            scheme.AddHeader("X-Email");
                        });
                    });

                    // Subscribe to HmacEvents to handle
                    // successes and failures
                    options.Events = new HmacEvents
                    {
                        OnValidateKeys = (context, keys) =>
                        {
                            // Validate keys against database
                            return true;
                        },
                        OnAuthenticationSuccess = (context, hmacResult) =>
                        {
                            var claims = new Claim[]
                            {
                                new Claim(ClaimTypes.Name, "MyName"),
                                new Claim(ClaimTypes.NameIdentifier, "MyNameId"),
                                new Claim(ClaimTypes.Email, "MyEmail")
                            };

                            return claims;
                        },
                        OnAuthenticationFailure = (context, hmacResult) => new Exception("An error occurred authenticating request.")
                    };
                });

                services.AddAuthorization(options => 
                    options.AddPolicy("TestPolicy", policy => 
                        policy.RequireHmacAuthentication("MyPolicy", "RequireAccountAndEmail")));
            })
            .Configure(app =>
            {
                app.UseHttpsRedirection();
                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/api", async (HttpContext context) =>
                    {
                        await context.Response.WriteAsync("Ok!");
                    }).RequireAuthorization("TestPolicy"); 

                    endpoints.MapPost("/api", async (HttpContext context, [FromBody] string body) =>
                    {
                        await context.Response.WriteAsync("Ok!");
                    }).RequireAuthorization("TestPolicy"); 
                });
            });

        // Create TestServer and HttpClient
        _server = new TestServer(builder);

        var hmacManagerFactory = _server.Services.GetRequiredService<IHmacManagerFactory>();
        var hmacManager = hmacManagerFactory.Create("MyPolicy", "RequireAccountAndEmail");
        var handler = new HmacDelegatingHandler(hmacManager)
        {
            InnerHandler = _server.CreateHandler()
        };

        _client = new HttpClient(handler)
        {
            BaseAddress = _server.BaseAddress
        };
    }

    [Test]
    public async Task Reading_Request_Body_Twice_Should_Throw_Error()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api")
        {
            Content = new StringContent(JsonConvert.SerializeObject("TestContent"), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("X-Account", "myAccount");
        request.Headers.Add("X-Email", "myEmail");

        var response = await _client.SendAsync(request);
        Assert.That(response.IsSuccessStatusCode);
    }

    [Test]
    public async Task Reading_Request_Body_Twice_Should_Get()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api");

        request.Headers.Add("X-Account", "myAccount");
        request.Headers.Add("X-Email", "myEmail");

        var response = await _client.SendAsync(request);
        Assert.That(response.IsSuccessStatusCode);
    }
}
