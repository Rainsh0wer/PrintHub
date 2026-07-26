using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Complaints.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class ComplaintsController : Controller
{
    private readonly PrintHubApiClient _api;
    public ComplaintsController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString(SessionKeys.UserRole) != "Customer")
            return RedirectToAction("Login", "Account", new { returnUrl = "/Complaints" });
        var res = await _api.GetAsync<PagedResult<ComplaintDto>>("/api/complaints/mine?PageSize=50");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new PagedResult<ComplaintDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Raise(int orderId, int reason, string description)
    {
        var res = await _api.PostAsync<ComplaintDto>("/api/complaints", new { orderId, reason, description });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Complaint submitted." : res.Error;
        return RedirectToAction("Details", "Orders", new { id = orderId });
    }

    [HttpPost] public Task<IActionResult> Accept(int id) => Act($"/api/complaints/{id}/accept", "Resolution accepted.");
    [HttpPost] public Task<IActionResult> Escalate(int id) => Act($"/api/complaints/{id}/escalate", "Complaint escalated to the platform.");

    private async Task<IActionResult> Act(string path, string ok)
    {
        var res = await _api.PutAsync<ComplaintDto>(path, null);
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? ok : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
