using System.Text;
using HmacManager.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services
    .AddHttpClient("Hmac_MyPolicy_RequiredAccountAndEmail", client =>
    {
        client.BaseAddress = new Uri("https://localhost:7216");
    }).AddHmacHttpMessageHandler("MyPolicy", "RequiredAccountAndEmail");

builder.Services
    .AddHmacManager(options =>
    {
        options.AddPolicy("MyPolicy", policy =>
        {
            policy.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolPrivateKey")));
            policy.UseMemoryCache(TimeSpan.FromSeconds(30));
            policy.AddScheme("RequireAccountAndEmail", scheme =>
            {
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
