using System.Text;
using HmacManager.Mvc.Extensions;
using Web.Services;

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
    .AddHttpClient("HmacPolicy_2_HmacScheme_2", client =>
    {
        client.BaseAddress = new Uri("https://localhost:7075");
    }).AddHmacHttpMessageHandler("HmacPolicy_2", "HmacScheme_2");

// Add HmacManager and create a policy
builder.Services
    .AddHmacManager(options =>
    {
        options.AddPolicy("HmacPolicy_1", policy =>
        {
            policy.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolHmacPolicy_1PrivateKey")));
            policy.UseMemoryCache(30);
            policy.AddScheme("HmacScheme_1", scheme =>
            {
                scheme.AddHeader("X-Scheme_1");
                scheme.AddHeader("X-Scheme_2");
            });
        });

        options.AddPolicy("HmacPolicy_2", policy =>
        {
            policy.UsePublicKey(Guid.Parse("ac2f1dae-08bd-4883-80fe-1d9a103b30b5"));
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolHmacPolicy_2PrivateKey")));
            policy.UseMemoryCache(30);
            policy.AddScheme("HmacScheme_2", scheme =>
            {
                scheme.AddHeader("X-Scheme_1");
                scheme.AddHeader("X-Scheme_2");
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