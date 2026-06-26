using HmacManager.Kubernetes;
using HmacManager.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHmacManager(builder.Configuration.GetSection("HmacManager"));
builder.Services.AddScoped<ExtAuthzHandler>();
builder.Services.AddScoped<SignHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapPost("/sign", async (SignRequest request, SignHandler handler) =>
        await handler.SignAsync(request));
}

// Catch-all: Envoy HTTP ext-authz forwards the original request's method and path
// to the ext-authz service, so this must match any method and any path.
// More-specific routes (e.g. /sign) take precedence over this catch-all.
string[] allMethods =
[
    HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete,
    HttpMethods.Patch, HttpMethods.Head, HttpMethods.Options
];
app.MapMethods("/{**path}", allMethods, async (HttpContext ctx, ExtAuthzHandler handler) =>
    await handler.CheckAsync(ctx));

app.Run();
