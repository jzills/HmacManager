using System.Text;
using Web.Services;
using HmacManager.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Add the weather service which is used to demo
// outgoing hmac requests
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Register a HttpClient and attach a HttpMessageHandler with
// the AddHmacHttpMessageHandler extension method. This accepts
// a policy and or scheme combination which will be used
// to sign outgoing requests
builder.Services
    .AddHttpClient("Hmac_MyPolicy_RequireAccountAndEmail", client =>
    {
        client.BaseAddress = new Uri("https://localhost:7216");
    }).AddHmacHttpMessageHandler("MyPolicy", "RequireAccountAndEmail");

// Add HmacManager and create a policy
builder.Services
    .AddHmacManager(options =>
    {
        options.AddPolicy("MyPolicy", policy =>
        {
            policy.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolPrivateKey")));

            // The max age is used to determine how long a request
            // is valid for, the nonce that is generated internally and
            // signed into the hmac will be stored in the cache registered here,
            // in this case in memory. The entry will be stored for the max age
            // of the request and then evicted
            policy.UseMemoryCache(TimeSpan.FromSeconds(30));

            // Create a scheme with required headers
            policy.AddScheme("RequireAccountAndEmail", scheme =>
            {
                // These are parsed automatically and added to claims
                // when using AddHmac from the AuthenticationBuilder
                // to verify incoming requests
                scheme.AddHeader("X-Account");
                scheme.AddHeader("X-Email");
            });
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
