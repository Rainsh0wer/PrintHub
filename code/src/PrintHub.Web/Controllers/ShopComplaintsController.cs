using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Complaints.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class ShopComplaintsController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public ShopComplaintsController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsShop()) return RedirectToAction("Login", "Account", new { returnUrl = "/ShopComplaints" });
        var shopId = CurrentShopId();
        if (shopId is null) { ViewBag.NoShop = true; return View(new PagedResult<ComplaintDto>()); }

        var res = await _api.GetAsync<PagedResult<ComplaintDto>>($"/api/shops/{shopId}/complaints?PageSize=50");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new PagedResult<ComplaintDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Respond(int id, int proposedResolution, decimal? refundAmount, string? shopResponse)
    {
        var res = await _api.PutAsync<ComplaintDto>($"/api/complaints/{id}/respond", new
        {
            proposedResolution,
            refundAmount = proposedResolution == 1 ? refundAmount : null,   // amount only matters for a refund
            shopResponse
        });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Response sent to the customer." : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
