using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Infrastructure.Persistence;
using PrintHub.Infrastructure.Security;

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

        return services;
    }
}
