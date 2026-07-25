using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Catalog;

namespace PrintHub.Api.Controllers;

/// <summary>Public read of the platform's active service catalogue (used to build a quote).</summary>
[ApiController]
[Route("api/service-types")]
[AllowAnonymous]
[Produces("application/json")]
public class ServiceCatalogController : ControllerBase
{
    private readonly IServiceTypeAdminService _catalogue;

    public ServiceCatalogController(IServiceTypeAdminService catalogue) => _catalogue = catalogue;

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await _catalogue.ListAsync(ct);
        if (result.IsFailure) return result.ToActionResult();
        var active = result.Value!.Where(s => s.IsActive).ToList();
        return Ok(ApiResponse.Ok(active));
    }
}
