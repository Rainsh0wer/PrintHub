using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Wallet.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class WalletController : Controller
{
    private readonly PrintHubApiClient _api;

    public WalletController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString(SessionKeys.UserRole) != "Customer")
            return RedirectToAction("Login", "Account", new { returnUrl = "/Wallet" });

        var res = await _api.GetAsync<WalletLedgerDto>("/api/wallet/transactions?PageSize=30");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data);
    }

    [HttpPost]
    public async Task<IActionResult> TopUp(decimal amount)
    {
        var res = await _api.PostAsync<TopUpResponse>("/api/wallet/topup", new { amount });
        if (res.Ok && res.Data is not null)
            return View("TopUp", res.Data);

        TempData["err"] = res.Error ?? "Could not start the top-up.";
        return RedirectToAction(nameof(Index));
    }
}
