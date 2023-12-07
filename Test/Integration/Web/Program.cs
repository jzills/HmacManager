using System.Security.Claims;
using System.Text;
using HmacManagement.Mvc;
using HmacManagement.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services
    .AddHttpClient()
    .AddAuthentication()
    .AddHmac(options =>
    {
        options.AddPolicy("MyPolicy_1", policy =>
        {
            policy.UsePublicKey(Guid.Parse("eb8e9dae-08bd-4883-80fe-1d9a103b30b5"));
            policy.UsePrivateKey(Convert.ToBase64String(Encoding.UTF8.GetBytes("thisIsMySuperCoolPrivateKey")));
            policy.UseInMemoryCache(TimeSpan.FromSeconds(30));
            policy.AddScheme("AccountEmailScheme", scheme =>
            {
                scheme.AddHeader("X-Account-Id", "AccountId");
                scheme.AddHeader("X-Email", "Email");
            });
        });

        options.Events = new HmacEvents
        {
            OnAuthenticationSuccess = context =>
            {
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, "MyName"),
                    new Claim(ClaimTypes.NameIdentifier, "MyNameId"),
                    new Claim(ClaimTypes.Email, "MyEmail")
                };

                return claims;
            },
            OnAuthenticationFailure = context =>
            {
                return new Exception("My Test Exception");
            }
        };
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
