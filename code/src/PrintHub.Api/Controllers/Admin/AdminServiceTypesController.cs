using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Catalog;
using PrintHub.Application.Features.Catalog.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers.Admin;

/// <summary>Admin service catalogue (UC-39): list, add, update, deactivate service types.</summary>
[ApiController]
[Route("api/admin/service-types")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Produces("application/json")]
public class AdminServiceTypesController : ControllerBase
{
    private readonly IServiceTypeAdminService _service;

    public AdminServiceTypesController(IServiceTypeAdminService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
        => (await _service.ListAsync(ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceTypeRequest request, CancellationToken ct)
        => (await _service.CreateAsync(request, ct)).ToActionResult(StatusCodes.Status201Created, "Service type created.");

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateServiceTypeRequest request, CancellationToken ct)
        => (await _service.UpdateAsync(id, request, ct)).ToActionResult(successMessage: "Service type updated.");

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
        => (await _service.DeactivateAsync(id, ct)).ToActionResult(successMessage: "Service type deactivated.");
}
