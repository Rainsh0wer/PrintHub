using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Vouchers;
using PrintHub.Application.Features.Vouchers.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers.Admin;

/// <summary>Admin vouchers (UC-40): list, create, edit, deactivate.</summary>
[ApiController]
[Route("api/admin/vouchers")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Produces("application/json")]
public class AdminVouchersController : ControllerBase
{
    private readonly IVoucherAdminService _vouchers;

    public AdminVouchersController(IVoucherAdminService vouchers) => _vouchers = vouchers;

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
        => (await _vouchers.ListAsync(ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create(CreateVoucherRequest request, CancellationToken ct)
        => (await _vouchers.CreateAsync(request, ct)).ToActionResult(StatusCodes.Status201Created, "Voucher created.");

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateVoucherRequest request, CancellationToken ct)
        => (await _vouchers.UpdateAsync(id, request, ct)).ToActionResult(successMessage: "Voucher updated.");

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
        => (await _vouchers.DeactivateAsync(id, ct)).ToActionResult(successMessage: "Voucher deactivated.");
}
