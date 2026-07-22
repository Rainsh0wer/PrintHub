using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Services;
using PrintHub.Application.Features.Auth;
using PrintHub.Application.Features.Favourites;
using PrintHub.Application.Features.Orders;
using PrintHub.Application.Features.Quotes;
using PrintHub.Application.Features.Shops;

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

        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IShopCatalogService, ShopCatalogService>();
        services.AddScoped<IFavouriteService, FavouriteService>();
        services.AddScoped<IShopOnboardingService, ShopOnboardingService>();
        services.AddScoped<IShopAdminService, ShopAdminService>();
        services.AddScoped<IShopProfileService, ShopProfileService>();
        services.AddScoped<IRateCardService, RateCardService>();
        services.AddScoped<IShopStaffService, ShopStaffService>();
        services.AddScoped<IShopResourceService, ShopResourceService>();
        services.AddScoped<IQuoteService, QuoteService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IShopOrderService, ShopOrderService>();

        return services;
    }
}
