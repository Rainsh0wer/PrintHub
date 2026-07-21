using Microsoft.AspNetCore.Server.Kestrel.Core;
using PrintHub.QuoteEngine.Pricing;
using PrintHub.QuoteEngine.Services;

var builder = WebApplication.CreateBuilder(args);

// Serve gRPC over HTTP/2 plaintext (h2c) on a fixed local port so the API can
// call it without TLS setup during development.
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5090, listen => listen.Protocols = HttpProtocols.Http2);
});

builder.Services.AddGrpc();

// Strategy pattern: register one pricing strategy per pricing model.
builder.Services.AddSingleton<IPricingStrategy, PerPageStrategy>();
builder.Services.AddSingleton<IPricingStrategy, PerUnitStrategy>();
builder.Services.AddSingleton<IPricingStrategy, MaterialAndTimeStrategy>();

var app = builder.Build();

app.MapGrpcService<QuoteEstimatorService>();
app.MapGet("/", () => "PrintHub QuoteEngine (gRPC over h2c on :5090). Use a gRPC client.");

app.Run();
