using HmacManager.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Get the configuration section where the policy schema is defined.

// This does not have to be on a property called "HmacManager" so any
// section name is acceptable, i.e. "Authentication", "Hmac", etc...

// The important piece is that the schema matches an array of polciies.
var section = builder.Configuration.GetSection("HmacManager");

// Pass the configuration section instead of using the 
// builder overload.
builder.Services.AddHmacManager(section);

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
