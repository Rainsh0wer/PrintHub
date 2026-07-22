using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Api.OData;

/// <summary>
/// Builds the OData EDM. Only DTOs are exposed (never entities), so OData query
/// options operate on a safe, shaped surface.
/// </summary>
public static class EdmModelBuilder
{
    public static IEdmModel Build()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<ShopODataDto>("Shops");
        builder.EntitySet<OrderODataDto>("Orders");
        return builder.GetEdmModel();
    }
}
