using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Console;
using HmacManagement.Caching;
using HmacManagement.Components;
using HmacManagement.Policies;

var policies = new HmacPolicyCollection();
policies.Add("Policy1", options =>
{
    options.Keys.PublicKey = Guid.NewGuid();
    options.Keys.PrivateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("myPrivateKey"));
    options.Algorithms.ContentAlgorithm = ContentHashAlgorithm.SHA256;
    options.Algorithms.SigningAlgorithm = SigningHashAlgorithm.HMACSHA256;
    options.Nonce.MaxAge = TimeSpan.FromMinutes(1);
    options.Nonce.CacheName = "MemoryCache";
    options.HeaderSchemes.Add("Policy1_Scheme1", options =>
    {
        options.AddHeader("Header1");
        options.AddHeader("Header2");
    });
});

var caches = new NonceCacheCollection();
caches.Add("MemoryCache", new NonceCacheMock1());

var factory = new HmacManagerFactory(policies, caches);

var hmacManager = factory.Create("Policy1", "Policy1_Scheme1");

var request = new HttpRequestMessage(HttpMethod.Post, "/api/user/123")
{
    Content = new StringContent(JsonSerializer.Serialize(new { Id = 1, Name = "Joshua" }), new MediaTypeHeaderValue("application/json"))
};
request.Headers.Add("Header1", "Header1_Value");
request.Headers.Add("Header2", "Header2_Value");

var signingResult = await hmacManager.SignAsync(request);
var debug = signingResult;

var verificationResult = await hmacManager.VerifyAsync(request);
var debug2 = verificationResult;