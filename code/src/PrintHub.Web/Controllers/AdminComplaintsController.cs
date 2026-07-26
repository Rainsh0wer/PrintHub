using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Complaints.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class AdminComplaintsController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public AdminComplaintsController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { returnUrl = "/AdminComplaints" });
        var res = await _api.GetAsync<PagedResult<ComplaintDto>>("/api/admin/complaints?PageSize=50");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new PagedResult<ComplaintDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Adjudicate(int id, bool upholdRefund, decimal? refundAmount, string? adminRuling)
    {
        var res = await _api.PutAsync<ComplaintDto>($"/api/admin/complaints/{id}/adjudicate", new { upholdRefund, refundAmount, adminRuling });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Ruling recorded." : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
