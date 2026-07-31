using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Wallet.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

/// <summary>UC-21 admin side: match a bank transfer to a pending reference and credit the wallet.</summary>
public class AdminWalletController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public AdminWalletController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { returnUrl = "/AdminWallet" });
        var res = await _api.GetAsync<PagedResult<PendingTopUpDto>>("/api/admin/wallet/topups/pending?PageSize=50");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new PagedResult<PendingTopUpDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Confirm(string refCode, string? bankReference)
    {
        var res = await _api.PutAsync<WalletTransactionDto>(
            $"/api/admin/wallet/topups/{Uri.EscapeDataString(refCode)}/confirm", new { bankReference });
        TempData[res.Ok ? "ok" : "err"] = res.Ok
            ? $"Top-up {refCode} confirmed — the customer's wallet has been credited."
            : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
