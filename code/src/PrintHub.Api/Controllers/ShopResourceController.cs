using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Shops;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>
/// Machine registry and material stock (UC-28). Operational, so both the owner
/// and shop staff may act; scoping is enforced inside the service.
/// </summary>
[ApiController]
[Route("api/shops/{shopId:int}")]
[Authorize]
[Produces("application/json")]
public class ShopResourceController : ControllerBase
{
    private readonly IShopResourceService _resources;

    public ShopResourceController(IShopResourceService resources) => _resources = resources;

    // ---- Machines ----

    [HttpGet("machines")]
    public async Task<IActionResult> ListMachines(int shopId, CancellationToken ct)
        => (await _resources.ListMachinesAsync(shopId, ct)).ToActionResult();

    [HttpPost("machines")]
    public async Task<IActionResult> AddMachine(int shopId, AddMachineRequest request, CancellationToken ct)
        => (await _resources.AddMachineAsync(shopId, request, ct)).ToActionResult(StatusCodes.Status201Created, "Machine added.");

    [HttpPut("machines/{machineId:int}/status")]
    public async Task<IActionResult> UpdateMachineStatus(int shopId, int machineId, UpdateMachineStatusRequest request, CancellationToken ct)
        => (await _resources.UpdateMachineStatusAsync(shopId, machineId, request, ct)).ToActionResult(successMessage: "Machine status updated.");

    [HttpDelete("machines/{machineId:int}")]
    public async Task<IActionResult> DeleteMachine(int shopId, int machineId, CancellationToken ct)
        => (await _resources.DeleteMachineAsync(shopId, machineId, ct)).ToActionResult(successMessage: "Machine removed.");

    // ---- Materials ----

    [HttpGet("materials")]
    public async Task<IActionResult> ListMaterials(int shopId, CancellationToken ct)
        => (await _resources.ListMaterialsAsync(shopId, ct)).ToActionResult();

    [HttpPost("materials")]
    public async Task<IActionResult> AddMaterial(int shopId, AddMaterialRequest request, CancellationToken ct)
        => (await _resources.AddMaterialAsync(shopId, request, ct)).ToActionResult(StatusCodes.Status201Created, "Material added.");

    [HttpPut("materials/{materialId:int}/stock")]
    public async Task<IActionResult> UpdateMaterialStock(int shopId, int materialId, UpdateMaterialStockRequest request, CancellationToken ct)
        => (await _resources.UpdateMaterialStockAsync(shopId, materialId, request, ct)).ToActionResult(successMessage: "Material stock updated.");

    [HttpDelete("materials/{materialId:int}")]
    public async Task<IActionResult> DeleteMaterial(int shopId, int materialId, CancellationToken ct)
        => (await _resources.DeleteMaterialAsync(shopId, materialId, ct)).ToActionResult(successMessage: "Material removed.");
}
