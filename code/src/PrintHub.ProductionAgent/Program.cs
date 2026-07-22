using Microsoft.EntityFrameworkCore;
using PrintHub.Infrastructure.Messaging;
using PrintHub.Infrastructure.Persistence;
using PrintHub.ProductionAgent;

var builder = Host.CreateApplicationBuilder(args);

// The agent shares the database with the API (it drives orders directly) and reads
// the same broker settings. No ICurrentUser is registered — the DbContext stamps
// system (null) actors for agent-driven transitions.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PrintHubDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddHostedService<ProductionWorker>();

builder.Build().Run();
