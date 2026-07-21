using Microsoft.EntityFrameworkCore;
using PrintHub.Infrastructure;
using PrintHub.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Persistence (DbContext, repositories, unit of work).
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Apply migrations and seed demo data on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PrintHubDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);

    if (app.Environment.IsDevelopment())
    {
        Console.WriteLine(
            $"[seed] users={await db.Users.CountAsync()} " +
            $"shops={await db.Shops.CountAsync()} " +
            $"serviceTypes={await db.ServiceTypes.CountAsync()} " +
            $"rateCard={await db.ShopServices.CountAsync()} " +
            $"machines={await db.Machines.CountAsync()} " +
            $"materials={await db.Materials.CountAsync()} " +
            $"orders={await db.Orders.CountAsync()} " +
            $"reviews={await db.Reviews.CountAsync()} " +
            $"vouchers={await db.Vouchers.CountAsync()}");
    }
}

// A one-shot mode used to seed and verify without serving. Full API endpoints
// (auth, OData, formatters) are wired in the API phase.
if (args.Contains("--seed-only"))
    return;

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "PrintHub API is running.");

app.Run();
