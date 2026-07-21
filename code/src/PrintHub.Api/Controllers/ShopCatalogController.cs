using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Shops;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>
/// Public shop discovery (UC-09, UC-10) — browsable without authentication so
/// price transparency is visible before a visitor registers.
/// </summary>
[ApiController]
[Route("api/shops")]
[Produces("application/json")]
public class ShopCatalogController : ControllerBase
{
    private readonly IShopCatalogService _shops;

    public ShopCatalogController(IShopCatalogService shops) => _shops = shops;

    /// <summary>UC-09 — search the shop directory with filtering, sorting, and pagination.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] ShopSearchRequest request, CancellationToken ct)
        => (await _shops.SearchAsync(request, ct)).ToActionResult();

    /// <summary>UC-10 — a shop's public profile, rate card, machines, and reviews.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Detail(int id, CancellationToken ct)
        => (await _shops.GetDetailAsync(id, ct)).ToActionResult();
}
