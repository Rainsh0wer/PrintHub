using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Documents.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class DocumentsController : Controller
{
    private readonly PrintHubApiClient _api;
    public DocumentsController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString(SessionKeys.UserRole) != "Customer")
            return RedirectToAction("Login", "Account", new { returnUrl = "/Documents" });
        var res = await _api.GetAsync<PagedResult<DocumentDto>>("/api/documents?PageSize=50");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new PagedResult<DocumentDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, int? declaredPageCount, bool rightsDeclared)
    {
        if (file is null || file.Length == 0) { TempData["err"] = "Please choose a file."; return RedirectToAction(nameof(Index)); }
        var fields = new List<KeyValuePair<string, string>>
        {
            new("declaredPageCount", (declaredPageCount ?? 1).ToString()),
            new("rightsDeclared", rightsDeclared ? "true" : "false")
        };
        var res = await _api.PostFileAsync<DocumentDto>("/api/documents", file, fields);
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "File uploaded." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await _api.DeleteAsync($"/api/documents/{id}");
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "File deleted." : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
