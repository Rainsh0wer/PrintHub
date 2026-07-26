using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Catalog.Dtos;
using PrintHub.Application.Features.Quotes.Dtos;
using PrintHub.Application.Features.Vouchers.Dtos;
using PrintHub.Domain.Enums;
using PrintHub.Web.Models;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class QuotesController : Controller
{
    private readonly PrintHubApiClient _api;

    public QuotesController(PrintHubApiClient api) => _api = api;

    [HttpGet]
    public async Task<IActionResult> Index(int? shopId)
    {
        if (RedirectIfNotCustomer(shopId) is { } r) return r;
        return View(new QuoteVm { ShopId = shopId, Services = await LoadServicesAsync() });
    }

    [HttpPost]
    public async Task<IActionResult> Index(QuoteVm form)
    {
        if (RedirectIfNotCustomer(form.ShopId) is { } r) return r;
        form.Services = await LoadServicesAsync();

        var item = new CompareItemInput
        {
            ServiceTypeId = form.ServiceTypeId,
            Quantity = Math.Max(1, form.Quantity),
            PageCount = form.PageCount,
            ColorMode = (ColorMode)form.ColorMode,
            Sides = (Sides)form.Sides,
            EstimatedGrams = form.EstimatedGrams
        };
        var request = new CompareQuotesRequest
        {
            Items = new List<CompareItemInput> { item },
            ShopId = form.ShopId,
            SortBy = (QuoteSortBy)form.SortBy
        };

        var res = await _api.PostAsync<List<QuoteComparisonDto>>("/api/quotes/compare", request);
        if (!res.Ok) { form.Error = res.Error; return View(form); }

        form.Results = res.Data ?? new();
        if (!string.IsNullOrWhiteSpace(form.VoucherCode) && form.Results.Count > 0)
            await ApplyVoucherPreviewAsync(form);

        return View(form);
    }

    // UC-14 preview: try the voucher against each compared quote and record the discount.
    private async Task ApplyVoucherPreviewAsync(QuoteVm form)
    {
        var code = form.VoucherCode!.Trim();
        string? lastError = null;
        foreach (var q in form.Results!)
        {
            var vr = await _api.PostAsync<VoucherApplicationDto>(
                $"/api/quotes/{q.QuoteId}/apply-voucher", new ApplyVoucherRequest(code));
            if (vr.Ok && vr.Data is { Discount: > 0 }) form.VoucherDiscounts[q.QuoteId] = vr.Data.Discount;
            else lastError = vr.Error;
        }

        form.VoucherNote = form.VoucherDiscounts.Count > 0
            ? $"Voucher “{code.ToUpperInvariant()}” applied to {form.VoucherDiscounts.Count} shop(s)."
            : lastError ?? "This voucher can't be applied to these quotes.";
    }

    private async Task<List<ServiceTypeAdminDto>> LoadServicesAsync()
    {
        var res = await _api.GetAsync<List<ServiceTypeAdminDto>>("/api/service-types");
        return res.Data ?? new();
    }

    private IActionResult? RedirectIfNotCustomer(int? shopId)
    {
        var role = HttpContext.Session.GetString(SessionKeys.UserRole);
        if (role == "Customer") return null;
        var back = shopId is null ? "/Quotes" : $"/Quotes?shopId={shopId}";
        return RedirectToAction("Login", "Account", new { returnUrl = back });
    }
}
