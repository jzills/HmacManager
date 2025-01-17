using HmacManager.Components;
using HmacManager.Mvc;
using HmacManager.Mvc.Extensions;
using HmacManager.Policies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;

[TestFixture]
public class Test_HmacAuthenticationHandler_WithDelegatingHandler_ScopedPolicies
{
    TestServer Server;
    HttpClient Client;
    IHmacManager HmacManager;

    [SetUp]
    public void Setup()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services => 
            {
                services.AddRouting();
                services.AddMemoryCache();
                services.AddAuthentication().AddHmac(options =>
                {
                    options.EnableConsolidatedHeaders();
                    options.EnableScopedPolicies(serviceProvider =>
                    {
                        var builder = new HmacPolicyBuilder("MyPolicy");
                        builder.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
                        builder.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolPrivateKey")));
                        builder.UseMemoryCache(30);
                        builder.AddScheme("RequireAccountAndEmail", scheme =>
                        {
                            scheme.AddHeader("X-Account");
                            scheme.AddHeader("X-Email");
                        });

                        var policies = new HmacPolicyCollection();
                        policies.Add(builder.Build());

                        return policies;
                    });
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

        Server = new TestServer(builder);
        Client = Server.CreateClient();
        HmacManager = Server.Services.GetRequiredService<IHmacManagerFactory>()
            .Create("MyPolicy", "RequireAccountAndEmail")!;
    }

    [Test]
    public async Task Test_Get()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api");

        request.Headers.Add("X-Account", "myAccount");
        request.Headers.Add("X-Email", "myEmail");

        await HmacManager.SignAsync(request);

        var response = await Client.SendAsync(request);
        Assert.That(response.IsSuccessStatusCode);
    }

    [Test]
    public async Task Test_Post()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api")
        {
            Content = new StringContent(JsonConvert.SerializeObject("TestContent"), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("X-Account", "myAccount");
        request.Headers.Add("X-Email", "myEmail");

        await HmacManager.SignAsync(request);

        var response = await Client.SendAsync(request);
        Assert.That(response.IsSuccessStatusCode);
    }
}
