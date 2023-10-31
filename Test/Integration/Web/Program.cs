using System.Security.Claims;
using HmacManagement.Mvc;
using HmacManagement.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services
    .AddAuthentication()
    .AddHmac(options =>
    {
        // options.Policies.AddPolicy("Default", policy =>
        // {
        //     policy.Keys.PublicKey = Guid.Parse("4c59aec6-517c-47b0-a681-3c0251037416");
        //     policy.Keys.PrivateKey = "CKnebrN5WUmFdIZE01O3hA==";
        //     policy.HeaderSchemes.AddHeaderScheme("Scheme1", scheme =>
        //     {
        //         scheme.AddRequiredHeader("");
        //     });
        // });

        options.Policies.AddPolicy("MyFirstPolicy", policy =>
        {
            policy.HeaderSchemes.AddHeaderScheme("AccountEmailScheme", scheme =>
            {
                scheme.AddRequiredHeader("X-Account-Id", "AccountId");
                scheme.AddRequiredHeader("X-Email", "Email");
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
