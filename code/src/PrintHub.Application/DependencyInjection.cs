using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PrintHub.Application.Features.Auth;

namespace PrintHub.Application;

/// <summary>
/// Composition root for the Application layer. The API calls
/// <c>services.AddApplication()</c> to register AutoMapper profiles, validators,
/// and the use-case services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
