using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Reports.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class AdminReportsController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public AdminReportsController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { returnUrl = "/AdminReports" });
        var res = await _api.GetAsync<PlatformReportDto>("/api/reports/platform");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new PlatformReportDto());
    }
}
