using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Vouchers.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class AdminVouchersController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public AdminVouchersController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { returnUrl = "/AdminVouchers" });
        var res = await _api.GetAsync<List<VoucherAdminDto>>("/api/admin/vouchers");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new List<VoucherAdminDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Create(string code, string? name, int discountType, decimal discountValue,
        decimal minOrderAmount, int usageLimit, DateTime validFrom, DateTime validTo, int perUserLimit)
    {
        var body = new { code, name, discountType, discountValue, minOrderAmount, usageLimit, validFrom, validTo, perUserLimit };
        var res = await _api.PostAsync<VoucherAdminDto>("/api/admin/vouchers", body);
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Voucher created." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    /// <summary>UC-40 — edit a voucher. The code is immutable, so only the terms are sent.</summary>
    [HttpPost]
    public async Task<IActionResult> Update(int id, string? name, decimal discountValue, decimal minOrderAmount,
        decimal? maxDiscountAmount, int usageLimit, DateTime validFrom, DateTime validTo, bool isActive,
        int perUserLimit, string? description)
    {
        var res = await _api.PutAsync<VoucherAdminDto>($"/api/admin/vouchers/{id}", new
        {
            name, discountValue, minOrderAmount, maxDiscountAmount,
            usageLimit, validFrom, validTo, isActive, perUserLimit, description
        });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Voucher updated." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Deactivate(int id)
    {
        var res = await _api.DeleteAsync($"/api/admin/vouchers/{id}");
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Voucher deactivated." : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
