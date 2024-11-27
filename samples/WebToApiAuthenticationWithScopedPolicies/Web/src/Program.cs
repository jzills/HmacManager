using System.Text;
using HmacManager.Mvc.Extensions;
using HmacManager.Policies;
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
        options.EnableScopedPolicies(serviceProvider =>
        {
            // Create policy collection
            var policies = new HmacPolicyCollection();

            // Create policies
            var builder = new HmacPolicyBuilder("HmacPolicy_1");
            builder.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
            builder.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolHmacPolicy_1PrivateKey")));
            builder.UseMemoryCache(30);
            builder.AddScheme("HmacScheme_1", scheme =>
            {
                scheme.AddHeader("X-Scheme_1");
                scheme.AddHeader("X-Scheme_2");
            });
            
            // Add policy to the collection
            policies.Add(builder.Build());

            // Create policies
            builder = new HmacPolicyBuilder("HmacPolicy_2");
            builder.UsePublicKey(Guid.Parse("ac2f1dae-08bd-4883-80fe-1d9a103b30b5"));
            builder.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolHmacPolicy_2PrivateKey")));
            builder.UseMemoryCache(30);
            builder.AddScheme("HmacScheme_2", scheme =>
            {
                scheme.AddHeader("X-Scheme_1");
                scheme.AddHeader("X-Scheme_2");
            });
            
            // Add policy to the collection
            policies.Add(builder.Build());

            // Return the collection
            return policies;
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