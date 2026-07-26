using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Contracts.Quoting;
using PrintHub.Infrastructure.Email;
using PrintHub.Infrastructure.Messaging;
using PrintHub.Infrastructure.Persistence;
using PrintHub.Infrastructure.Quoting;
using PrintHub.Infrastructure.Security;
using PrintHub.Infrastructure.Storage;

namespace PrintHub.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer. The API calls
/// <c>services.AddInfrastructure(configuration)</c> to wire persistence and security.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<PrintHubDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(PrintHubDbContext).Assembly.FullName)));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Security (Options pattern for JWT settings).
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // gRPC Quote Engine client. Uses HTTP/2 plaintext (h2c) for local dev.
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        var quoteEngineAddress = configuration["QuoteEngine:Address"] ?? "http://localhost:5090";
        services.AddGrpcClient<QuoteEstimator.QuoteEstimatorClient>(o => o.Address = new Uri(quoteEngineAddress));
        services.AddScoped<IQuoteEngineClient, QuoteEngineClient>();

        // Async production pipeline (RabbitMQ publisher; degrades gracefully if the broker is down).
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<IProductionQueue, RabbitMqProductionQueue>();

        // File storage: Cloudinary when configured, else local disk (graceful default).
        services.Configure<CloudinaryOptions>(configuration.GetSection(CloudinaryOptions.SectionName));
        var cloudinary = configuration.GetSection(CloudinaryOptions.SectionName).Get<CloudinaryOptions>();
        if (cloudinary?.IsConfigured == true)
            services.AddSingleton<IFileStorage, CloudinaryFileStorage>();
        else
            services.AddSingleton<IFileStorage, LocalFileStorage>();

        // Email (SMTP via MailKit); no-ops when the section is blank.
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
