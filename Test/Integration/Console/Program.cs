using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Console;
using HmacManagement.Caching;
using HmacManagement.Components;
using HmacManagement.Policies;
using HmacManagement.Mvc.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMemoryCache();
services.AddAuthentication()
    .AddHmac(options =>
    {
        options.AddPolicy("MyFirstPolicy", policy =>
        {
            policy.UsePublicKey(Guid.NewGuid());
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("mySuperLongString")));
            policy.UseContentHashAlgorithm(ContentHashAlgorithm.SHA256);
            policy.UseSigningHashAlgorithm(SigningHashAlgorithm.HMACSHA256);
            policy.UseInMemoryCache(TimeSpan.FromMinutes(1));

            policy.AddScheme("MyScheme1", scheme =>
            {
                scheme.AddHeader("MyHeaderForScheme1.1");
                scheme.AddHeader("MyHeaderForScheme1.2");
            });

            policy.AddScheme("MyScheme2", scheme =>
            {
                scheme.AddHeader("MyHeaderForScheme2.1");
                scheme.AddHeader("MyHeaderForScheme2.2");
            });
        });
    });

var serviceProvider = services.BuildServiceProvider();
var hmacFactory = serviceProvider.GetRequiredService<IHmacManagerFactory>();
var hmacManager = hmacFactory.Create("MyFirstPolicy", "MyScheme");
var debug = hmacManager;

// var policies = new HmacPolicyCollection();
// policies.Add("Policy1", options =>
// {
//     // options.UseKeys("", "");
//     // options.UseContentHashAlgorithm(ContentHashAlgorithm.SHA256);
//     // options.UseSigningHashAlgorithm(SigningHashAlgorithm.HMACSHA256);
//     // options.UseNonce("MemoryCache", TimeSpan.FromMinutes(1));
//     // options.AddScheme()

//     // options.HeaderSchemes.Add("Policy1_Scheme1", options =>
//     // {
//     //     options.AddHeader("Header1");
//     //     options.AddHeader("Header2");
//     // });
    
//     options.Keys.PublicKey = Guid.NewGuid();
//     options.Keys.PrivateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("myPrivateKey"));
//     options.Algorithms.ContentHashAlgorithm = ContentHashAlgorithm.SHA256;
//     options.Algorithms.SigningHashAlgorithm = SigningHashAlgorithm.HMACSHA256;
//     options.Nonce.MaxAge = TimeSpan.FromMinutes(1);
//     options.Nonce.CacheName = "MemoryCache";
//     options.HeaderSchemes.Add("Policy1_Scheme1", options =>
//     {
//         options.AddHeader("Header1");
//         options.AddHeader("Header2");
//     });
// });

// var caches = new NonceCacheCollection();
// caches.Add("MemoryCache", new NonceCacheMock1());

// var factory = new HmacManagerFactory(policies, caches);

// var hmacManager = factory.Create("Policy1", "Policy1_Scheme1");

// var request = new HttpRequestMessage(HttpMethod.Post, "/api/user/123")
// {
//     Content = new StringContent(JsonSerializer.Serialize(new { Id = 1, Name = "Joshua" }), new MediaTypeHeaderValue("application/json"))
// };
// request.Headers.Add("Header1", "Header1_Value");
// request.Headers.Add("Header2", "Header2_Value");

// var signingResult = await hmacManager.SignAsync(request);
// var debug = signingResult;

// var verificationResult = await hmacManager.VerifyAsync(request);
// var debug2 = verificationResult;