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
// MapFallback matches all methods and any unmatched path, so /sign takes precedence.
app.MapFallback(async (HttpContext ctx, ExtAuthzHandler handler) =>
    await handler.CheckAsync(ctx));

app.Run();
